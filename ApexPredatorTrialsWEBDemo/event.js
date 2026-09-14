const output = document.getElementById('output');
function showOutput(data) { output.textContent = JSON.stringify(data, null, 2); }

function getEventIdFromUrl() {
  const params = new URLSearchParams(window.location.search);
  return params.get('id');
}

const eventId = getEventIdFromUrl();
let currentEvent = null;
let currentBracket = [];

function getFinalSlot() {
  return currentBracket.find((s) => s.round === 'Final');
}

function championName(finalSlot) {
  if (!finalSlot || !finalSlot.winnerPlayerId) return null;
  return finalSlot.winnerPlayerId === finalSlot.hunterPlayerId ? finalSlot.hunterPlayerName : finalSlot.humanPlayerName;
}

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

function renderConcludeSection(section) {
  const finalSlot = getFinalSlot();
  if (!finalSlot) {
    section.innerHTML = '<p>No Final matchup exists yet.</p>';
    return;
  }

  const champion = championName(finalSlot);
  if (champion) {
    section.innerHTML = `<h3>&#127942; ${champion} won!</h3>`;
    return;
  }

  const heading = document.createElement('h3');
  heading.textContent = 'Declare Final winner';
  section.appendChild(heading);

  const form = document.createElement('form');
  form.id = 'conclude-form';

  const fieldset = document.createElement('fieldset');
  const legend = document.createElement('legend');
  legend.innerHTML = matchupLabel(finalSlot);
  fieldset.appendChild(legend);

  [['hunter', finalSlot.hunterPlayerId, finalSlot.hunterPlayerName], ['human', finalSlot.humanPlayerId, finalSlot.humanPlayerName]].forEach(([side, playerId, playerName]) => {
    const label = document.createElement('label');
    const radio = document.createElement('input');
    radio.type = 'radio';
    radio.name = 'final-winner';
    radio.value = String(playerId);
    radio.required = true;
    radio.disabled = !playerId;
    label.appendChild(radio);
    label.append(` ${playerName || 'TBD'} (${side})`);
    fieldset.appendChild(label);
  });
  form.appendChild(fieldset);

  const submitBtn = document.createElement('button');
  submitBtn.type = 'submit';
  submitBtn.textContent = 'Conclude event';
  form.appendChild(submitBtn);

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    const selected = form.querySelector('input[name="final-winner"]:checked');
    if (!selected) {
      showOutput({ error: 'Select the Final winner before concluding.' });
      return;
    }
    try {
      await apiFetch(`/events/${eventId}/conclude`, {
        method: 'POST',
        body: JSON.stringify({ winnerPlayerId: Number(selected.value) })
      });
      showOutput({ message: 'Event concluded.' });
      loadEvent();
    } catch (err) {
      showOutput({ error: err.message });
    }
  });

  section.appendChild(form);
}

function renderAdvancePhaseSection() {
  const section = document.getElementById('advance-phase-section');
  section.innerHTML = '';

  if (currentEvent.currentRound === 'Final') {
    renderConcludeSection(section);
    return;
  }

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
  const maxPairings = currentSlots.length / 2;
  const pairingHint = document.createElement('p');
  pairingHint.textContent = `Pair up the declared winners into ${nextRound} matchups (one Hunter side, one Human side). ${currentSlots.length} winners means exactly ${maxPairings} ${maxPairings === 1 ? 'matchup' : 'matchups'} next round.`;
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

  // Every current-round winner must advance into exactly one next-round matchup, so with N winners
  // there are always exactly N/2 pairings possible — no manual "add pairing" beyond that.
  for (let i = 0; i < maxPairings; i++) renderPairingRow();

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

function renderManagePanel() {
  document.getElementById('admin-panel').style.display = '';
  renderEditEventForm();
  renderAdvancePhaseSection();

  const deleteBtn = document.getElementById('delete-event-btn');
  // Only Admins may delete an event; the organizer can manage everything else about their own event.
  deleteBtn.style.display = isAdmin() ? '' : 'none';
  deleteBtn.onclick = async () => {
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

    currentBracket = await apiFetch(`/events/${eventId}/bracket`);
    renderBracket(currentBracket);

    const champion = championName(getFinalSlot());
    const statusLine = champion
      ? `<p><strong>Event concluded</strong></p>`
      : `<p><strong>Current round:</strong> ${currentEvent.currentRound}</p>`;
    document.getElementById('event-summary').innerHTML = `
      <p><strong>Region:</strong> ${currentEvent.region}</p>
      <p><strong>Dates:</strong> ${currentEvent.startDate} - ${currentEvent.endDate}</p>
      ${statusLine}
    `;

    if (canManageEvent(currentEvent)) renderManagePanel();
  } catch (err) {
    document.getElementById('event-summary').textContent = `Failed to load event: ${err.message}`;
  }
}

renderHeader();
loadEvent();
