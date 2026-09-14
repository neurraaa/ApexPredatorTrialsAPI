const output = document.getElementById('output');
function showOutput(data) { output.textContent = JSON.stringify(data, null, 2); }

function getEventIdFromUrl() {
  const params = new URLSearchParams(window.location.search);
  return params.get('id');
}

const eventId = getEventIdFromUrl();
let currentEvent = null;
let currentBracket = [];
let availableMaps = [];

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

function resultField(form, labelText, className, value) {
  const label = document.createElement('label');
  label.textContent = ` ${labelText}: `;
  const input = document.createElement('input');
  input.type = 'number';
  input.className = className;
  input.min = 0;
  input.value = value;
  input.required = true;
  label.appendChild(input);
  form.appendChild(label);
  return input;
}

function renderMatchResultForm(container, slot, onSaved, existing) {
  container.innerHTML = '';
  const heading = document.createElement('h4');
  heading.innerHTML = matchupLabel(slot);
  container.appendChild(heading);

  const form = document.createElement('form');

  const winnerLabel = document.createElement('label');
  winnerLabel.textContent = 'Winner: ';
  const winnerSelect = document.createElement('select');
  [['hunter', slot.hunterPlayerId, slot.hunterPlayerName], ['human', slot.humanPlayerId, slot.humanPlayerName]].forEach(([side, id, name]) => {
    const opt = document.createElement('option');
    opt.value = id;
    opt.dataset.side = side;
    opt.textContent = `${name} (${side})`;
    if (existing && existing.winnerId === id) opt.selected = true;
    winnerSelect.appendChild(opt);
  });
  winnerLabel.appendChild(winnerSelect);
  form.appendChild(winnerLabel);

  // Loser deaths/kills are always the mirror of winner kills/deaths, so only the winner's
  // stats are entered. Hunter wins by reaching 10 kills; Human wins by destroying 5 nests.
  const winnerDeaths = resultField(form, 'Winner deaths', 'result-winner-deaths', existing ? existing.winnerDeaths : 0);
  const winnerKills = resultField(form, 'Winner kills', 'result-winner-kills', existing ? existing.winnerKills : 0);
  const nests = resultField(form, 'Nests destroyed', 'result-nests', existing ? existing.nestsDestroyed : 0);
  nests.max = 5;

  function applyWinCondition() {
    const side = winnerSelect.options[winnerSelect.selectedIndex].dataset.side;
    if (side === 'hunter') winnerKills.value = 10;
    if (side === 'human') nests.value = 5;
  }
  winnerSelect.addEventListener('change', applyWinCondition);
  if (!existing) applyWinCondition();

  const submitBtn = document.createElement('button');
  submitBtn.type = 'submit';
  submitBtn.textContent = existing ? 'Save changes' : 'Save result';
  form.appendChild(submitBtn);

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    const winnerId = Number(winnerSelect.value);
    const loserId = winnerId === slot.hunterPlayerId ? slot.humanPlayerId : slot.hunterPlayerId;
    try {
      await apiFetch(`/events/${eventId}/schedules/${slot.id}/results`, {
        method: 'PUT',
        body: JSON.stringify({
          winnerId,
          loserId,
          winnerDeaths: Number(winnerDeaths.value),
          winnerKills: Number(winnerKills.value),
          loserDeaths: Number(winnerKills.value),
          loserKills: Number(winnerDeaths.value),
          nestsDestroyed: Number(nests.value)
        })
      });
      showOutput({ message: 'Match result saved.' });
      onSaved();
    } catch (err) {
      showOutput({ error: err.message });
    }
  });

  container.appendChild(form);
}

function renderMatchResultSection(container, slot, onSaved) {
  container.innerHTML = '';
  const heading = document.createElement('h4');
  heading.innerHTML = matchupLabel(slot);
  container.appendChild(heading);

  if (!slot.winnerPlayerId) {
    renderMatchResultForm(container, slot, onSaved, null);
    return;
  }

  const winnerName = slot.winnerPlayerId === slot.hunterPlayerId ? slot.hunterPlayerName : slot.humanPlayerName;
  const p = document.createElement('p');
  p.innerHTML = `&#127942; Winner: <strong>${winnerName}</strong> &mdash; <a href="match.html?id=${slot.matchId}">view full result</a>`;
  container.appendChild(p);

  const editBtn = document.createElement('button');
  editBtn.type = 'button';
  editBtn.textContent = 'Edit result';
  editBtn.addEventListener('click', async () => {
    let existing = null;
    try {
      existing = await apiFetch(`/match-results/by-match/${slot.matchId}`);
    } catch {
      // Fall back to a blank form if the existing result can't be loaded.
    }
    renderMatchResultForm(container, slot, onSaved, existing);
  });
  container.appendChild(editBtn);
}

function renderAdvancePhaseSection() {
  const section = document.getElementById('advance-phase-section');
  section.innerHTML = '';

  const currentSlots = currentBracket.filter((s) => s.round === currentEvent.currentRound);
  if (currentSlots.length === 0) {
    section.innerHTML = '<p>No matchups in the current round yet.</p>';
    return;
  }

  const resultsHeading = document.createElement('h3');
  resultsHeading.textContent = 'Match results';
  section.appendChild(resultsHeading);

  currentSlots.forEach((slot) => {
    const slotContainer = document.createElement('div');
    section.appendChild(slotContainer);
    renderMatchResultSection(slotContainer, slot, loadEvent);
  });

  const allResultsRecorded = currentSlots.every((s) => !!s.winnerPlayerId);

  if (currentEvent.currentRound === 'Final') {
    if (allResultsRecorded) {
      const p = document.createElement('p');
      p.innerHTML = '<strong>Event concluded.</strong>';
      section.appendChild(p);
    }
    return;
  }

  const nextRoundIndex = ROUND_ORDER.indexOf(currentEvent.currentRound) + 1;
  if (nextRoundIndex <= 0 || nextRoundIndex >= ROUND_ORDER.length) return;
  const nextRound = ROUND_ORDER[nextRoundIndex];

  if (!allResultsRecorded) {
    const p = document.createElement('p');
    p.textContent = `Record every ${currentEvent.currentRound} match result before advancing to ${nextRound}.`;
    section.appendChild(p);
    return;
  }

  const heading = document.createElement('h3');
  heading.textContent = `Advance to ${nextRound}`;
  section.appendChild(heading);

  const form = document.createElement('form');
  form.id = 'advance-phase-form';

  const pairingHeading = document.createElement('h4');
  pairingHeading.textContent = 'Next round pairings';
  const maxPairings = currentSlots.length / 2;
  const pairingHint = document.createElement('p');
  pairingHint.textContent = `Pair up the declared winners into ${nextRound} matchups (one Hunter side, one Human side), and pick a map for each. ${currentSlots.length} winners means exactly ${maxPairings} ${maxPairings === 1 ? 'matchup' : 'matchups'} next round.`;
  form.appendChild(pairingHeading);
  form.appendChild(pairingHint);

  const pairingsList = document.createElement('div');
  form.appendChild(pairingsList);

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
    const mapSelect = document.createElement('select');
    availableMaps.forEach((m) => {
      const opt = document.createElement('option');
      opt.value = m.id;
      opt.textContent = m.name;
      mapSelect.appendChild(opt);
    });

    row.appendChild(document.createTextNode('Hunter-side winner from: '));
    row.appendChild(hunterSelect);
    row.appendChild(document.createTextNode(' Human-side winner from: '));
    row.appendChild(humanSelect);
    row.appendChild(document.createTextNode(' Map: '));
    row.appendChild(mapSelect);
    pairingsList.appendChild(row);
    pairings.push({ hunterSelect, humanSelect, mapSelect });
  }

  // Every current-round winner must advance into exactly one next-round matchup, so with N winners
  // there are always exactly N/2 pairings possible — no manual "add pairing" beyond that.
  for (let i = 0; i < maxPairings; i++) renderPairingRow();

  const submitBtn = document.createElement('button');
  submitBtn.type = 'submit';
  submitBtn.textContent = `Advance to ${nextRound}`;
  form.appendChild(submitBtn);

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    try {
      const nextMatchups = pairings.map((p) => ({
        hunterScheduleId: Number(p.hunterSelect.value),
        humanScheduleId: Number(p.humanSelect.value),
        mapId: Number(p.mapSelect.value)
      }));

      await apiFetch(`/events/${eventId}/advance-phase`, {
        method: 'POST',
        body: JSON.stringify({ nextRound, nextMatchups })
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

    if (canManageEvent(currentEvent)) {
      try {
        availableMaps = await apiFetch('/maps');
      } catch {
        availableMaps = [];
      }
      renderManagePanel();
    }
  } catch (err) {
    document.getElementById('event-summary').textContent = `Failed to load event: ${err.message}`;
  }
}

renderHeader();
loadEvent();
