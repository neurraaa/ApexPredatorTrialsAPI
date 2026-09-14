const output = document.getElementById('output');
function showOutput(data) { output.textContent = JSON.stringify(data, null, 2); }

function getEventIdFromUrl() {
  const params = new URLSearchParams(window.location.search);
  return params.get('id');
}

const eventId = getEventIdFromUrl();
let currentEvent = null;
let currentBracket = [];

function renderBracket(bracket) {
  const container = document.getElementById('bracket-container');
  container.innerHTML = '';
  if (bracket.length === 0) {
    container.textContent = 'No bracket set up yet for this event.';
    return;
  }

  const columns = document.createElement('div');
  columns.style.display = 'flex';
  columns.style.gap = '2rem';

  ROUND_ORDER.forEach((round) => {
    const slots = bracket.filter((s) => s.round === round);
    if (slots.length === 0) return;

    const column = document.createElement('div');
    column.innerHTML = `<h3>${round}</h3>`;
    const list = document.createElement('ul');
    slots.forEach((slot) => {
      const li = document.createElement('li');
      let label = matchupLabel(slot);
      if (slot.winnerPlayerId) {
        const winnerName = slot.winnerPlayerId === slot.hunterPlayerId ? slot.hunterPlayerName : slot.humanPlayerName;
        label += ` &mdash; Winner: <strong>${winnerName}</strong>`;
      }
      li.innerHTML = label;
      list.appendChild(li);
    });
    column.appendChild(list);
    columns.appendChild(column);
  });

  container.appendChild(columns);
}

function renderEditEventForm() {
  const section = document.getElementById('edit-event-section');
  section.innerHTML = `
    <h3>Edit event</h3>
    <form id="edit-event-form">
      <input type="text" id="edit-title" value="${currentEvent.title}" required>
      <select id="edit-region">
        ${['EU', 'NA', 'SA', 'AP'].map((r) => `<option value="${r}" ${r === currentEvent.region ? 'selected' : ''}>${r}</option>`).join('')}
      </select>
      <label>Start: <input type="date" id="edit-start" value="${currentEvent.startDate.slice(0, 10)}" required></label>
      <label>End: <input type="date" id="edit-end" value="${currentEvent.endDate.slice(0, 10)}" required></label>
      <button type="submit">Save</button>
    </form>
  `;

  document.getElementById('edit-event-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    try {
      await apiFetch(`/events/${eventId}`, {
        method: 'PUT',
        body: JSON.stringify({
          title: document.getElementById('edit-title').value,
          region: document.getElementById('edit-region').value,
          startDate: document.getElementById('edit-start').value,
          endDate: document.getElementById('edit-end').value
        })
      });
      showOutput({ message: 'Event updated.' });
      loadEvent();
    } catch (err) {
      showOutput({ error: err.message });
    }
  });
}

function renderAdvancePhaseSection() {
  const section = document.getElementById('advance-phase-section');
  section.innerHTML = '';

  const nextRoundIndex = ROUND_ORDER.indexOf(currentEvent.currentRound) + 1;
  if (nextRoundIndex >= ROUND_ORDER.length || nextRoundIndex <= 0) {
    section.innerHTML = '<p>This event has reached the Final.</p>';
    return;
  }
  const nextRound = ROUND_ORDER[nextRoundIndex];

  const currentSlots = currentBracket.filter((s) => s.round === currentEvent.currentRound);
  if (currentSlots.length === 0) {
    section.innerHTML = '<p>No matchups in the current round yet.</p>';
    return;
  }

  const heading = document.createElement('h3');
  heading.textContent = `Advance to ${nextRound}`;
  section.appendChild(heading);

  const form = document.createElement('form');
  form.id = 'advance-phase-form';

  const winnerSelections = {};
  currentSlots.forEach((slot) => {
    const fieldset = document.createElement('fieldset');
    const legend = document.createElement('legend');
    legend.innerHTML = matchupLabel(slot);
    fieldset.appendChild(legend);

    [['hunter', slot.hunterPlayerId, slot.hunterPlayerName], ['human', slot.humanPlayerId, slot.humanPlayerName]].forEach(([side, playerId, playerName]) => {
      const label = document.createElement('label');
      const radio = document.createElement('input');
      radio.type = 'radio';
      radio.name = `winner-${slot.id}`;
      radio.value = String(playerId);
      radio.required = true;
      radio.disabled = !playerId;
      radio.addEventListener('change', () => { winnerSelections[slot.id] = Number(playerId); });
      label.appendChild(radio);
      label.append(` ${playerName || 'TBD'} (${side})`);
      fieldset.appendChild(label);
    });
    form.appendChild(fieldset);
  });

  const pairingContainer = document.createElement('div');
  const pairingHeading = document.createElement('h4');
  pairingHeading.textContent = 'Next round pairings';
  const pairingHint = document.createElement('p');
  pairingHint.textContent = `Pair up the declared winners into ${nextRound} matchups (one Hunter side, one Human side).`;
  pairingContainer.appendChild(pairingHeading);
  pairingContainer.appendChild(pairingHint);

  const pairingsList = document.createElement('div');
  pairingContainer.appendChild(pairingsList);

  const pairings = [];
  function renderPairingRow() {
    const row = document.createElement('div');
    const hunterSelect = document.createElement('select');
    const humanSelect = document.createElement('select');
    currentSlots.forEach((slot) => {
      [hunterSelect, humanSelect].forEach((select) => {
        const opt = document.createElement('option');
        opt.value = slot.id;
        opt.textContent = matchupLabel(slot).replace(/<[^>]+>/g, '');
        select.appendChild(opt);
      });
    });
    row.appendChild(document.createTextNode('Hunter-side winner from: '));
    row.appendChild(hunterSelect);
    row.appendChild(document.createTextNode(' Human-side winner from: '));
    row.appendChild(humanSelect);
    pairingsList.appendChild(row);
    pairings.push({ hunterSelect, humanSelect });
  }

  const addPairingBtn = document.createElement('button');
  addPairingBtn.type = 'button';
  addPairingBtn.textContent = 'Add pairing';
  addPairingBtn.addEventListener('click', renderPairingRow);
  pairingContainer.appendChild(addPairingBtn);

  for (let i = 0; i < Math.ceil(currentSlots.length / 2); i++) renderPairingRow();

  form.appendChild(pairingContainer);

  const submitBtn = document.createElement('button');
  submitBtn.type = 'submit';
  submitBtn.textContent = `Advance to ${nextRound}`;
  form.appendChild(submitBtn);

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    try {
      const winners = currentSlots.map((slot) => ({
        scheduleId: slot.id,
        winnerPlayerId: winnerSelections[slot.id]
      }));
      if (winners.some((w) => !w.winnerPlayerId)) {
        showOutput({ error: 'Declare a winner for every matchup before advancing.' });
        return;
      }
      const nextMatchups = pairings.map((p) => ({
        hunterScheduleId: Number(p.hunterSelect.value),
        humanScheduleId: Number(p.humanSelect.value)
      }));

      await apiFetch(`/events/${eventId}/advance-phase`, {
        method: 'POST',
        body: JSON.stringify({ nextRound, winners, nextMatchups })
      });
      showOutput({ message: `Advanced to ${nextRound}.` });
      loadEvent();
    } catch (err) {
      showOutput({ error: err.message });
    }
  });

  section.appendChild(form);
}

function renderAdminPanel() {
  document.getElementById('admin-panel').style.display = '';
  renderEditEventForm();
  renderAdvancePhaseSection();

  document.getElementById('delete-event-btn').onclick = async () => {
    if (!confirm(`Delete event "${currentEvent.title}"?`)) return;
    try {
      await apiFetch(`/events/${eventId}`, { method: 'DELETE' });
      window.location.href = 'index.html';
    } catch (err) {
      showOutput({ error: err.message });
    }
  };
}

async function loadEvent() {
  if (!eventId) {
    document.getElementById('event-summary').textContent = 'No event id given. Open this page as event.html?id=1';
    return;
  }

  try {
    currentEvent = await apiFetch(`/events/${eventId}`);
    document.getElementById('event-title').textContent = currentEvent.title;
    document.getElementById('event-summary').innerHTML = `
      <p><strong>Region:</strong> ${currentEvent.region}</p>
      <p><strong>Dates:</strong> ${currentEvent.startDate} - ${currentEvent.endDate}</p>
      <p><strong>Current round:</strong> ${currentEvent.currentRound}</p>
    `;

    currentBracket = await apiFetch(`/events/${eventId}/bracket`);
    renderBracket(currentBracket);

    if (isAdmin()) renderAdminPanel();
  } catch (err) {
    document.getElementById('event-summary').textContent = `Failed to load event: ${err.message}`;
  }
}

renderHeader();
loadEvent();
