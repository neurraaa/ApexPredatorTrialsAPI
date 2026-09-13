const ROUND_ORDER = ['Quarter-Final', 'Semi-Final', 'Final'];

const output = document.getElementById('output');
function showOutput(data) { output.textContent = JSON.stringify(data, null, 2); }

function playerLink(id, name) {
  if (!id) return 'TBD';
  return `<a href="player.html?id=${id}">${name}</a>`;
}

function matchupLabel(slot) {
  const hunter = playerLink(slot.hunterPlayerId, slot.hunterPlayerName);
  const human = playerLink(slot.humanPlayerId, slot.humanPlayerName);
  return `${hunter} (Hunter) vs ${human} (Human)`;
}

function pickCurrentRound(slots) {
  for (const round of ROUND_ORDER) {
    const roundSlots = slots.filter((s) => s.round === round);
    if (roundSlots.length > 0 && roundSlots.some((s) => !s.matchId)) {
      return round;
    }
  }
  return ROUND_ORDER[ROUND_ORDER.length - 1];
}

async function registerForEvent(eventId) {
  try {
    const myPlayer = await apiFetch('/players/me');
    const result = await apiFetch('/event-registrations', {
      method: 'POST',
      body: JSON.stringify({ eventId, playerId: myPlayer.id })
    });
    showOutput(result);
  } catch (err) {
    showOutput({ error: err.message });
  }
}

async function renderEvent(event) {
  const wrapper = document.createElement('div');
  wrapper.innerHTML = `<h3>${event.title} (${event.region})</h3><p>${event.startDate} - ${event.endDate}</p>`;
 
  try {
    const bracket = await apiFetch(`/events/${event.id}/bracket`);
 
    if (bracket.length === 0) {
      wrapper.innerHTML += `<p>No bracket set up yet for this event.</p>`;
    } else {
      const currentRound = pickCurrentRound(bracket);
      const currentSlots = bracket.filter((s) => s.round === currentRound);
 
      const list = document.createElement('ul');
      currentSlots.forEach((slot) => {
        const li = document.createElement('li');
        // innerHTML, not textContent - matchupLabel returns real <a> tags
        // that need to actually render as links, not escaped text.
        li.innerHTML = `[${slot.round}] ${matchupLabel(slot)}`;
        list.appendChild(li);
      });
 
      const roundLabel = document.createElement('p');
      roundLabel.innerHTML = `<strong>Current round: ${currentRound}</strong>`;
      wrapper.appendChild(roundLabel);
      wrapper.appendChild(list);
    }
  } catch (err) {
    wrapper.innerHTML += `<p>Could not load bracket: ${err.message}</p>`;
  }
 
  if (getUser()) {
    const registerBtn = document.createElement('button');
    registerBtn.textContent = 'Register for this event';
    registerBtn.addEventListener('click', () => registerForEvent(event.id));
    wrapper.appendChild(registerBtn);
  }
 
  return wrapper;
}

async function loadEvents() {
  const container = document.getElementById('events-container');
  try {
    const events = await apiFetch('/events');
    container.innerHTML = '';
    if (events.length === 0) {
      container.textContent = 'No events yet.';
      return;
    }
    for (const event of events) {
      container.appendChild(await renderEvent(event));
    }
  } catch (err) {
    container.textContent = `Failed to load events: ${err.message}`;
  }
}

function renderCreateEventForm() {
  const placeholder = document.getElementById('create-event-placeholder');
  if (!getUser()) return;
 
  placeholder.innerHTML = `
    <h2>Create Event</h2>
    <form id="create-event-form">
      <input type="text" id="event-title" placeholder="Title" required>
      <select id="event-region">
        <option value="EU">EU</option>
        <option value="NA">NA</option>
        <option value="SA">SA</option>
        <option value="AP">AP</option>
      </select>
      <label>Start: <input type="date" id="event-start" required></label>
      <label>End: <input type="date" id="event-end" required></label>
      <button type="submit">Create Event</button>
    </form>
  `;
 
  document.getElementById('create-event-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    try {
      const result = await apiFetch('/events', {
        method: 'POST',
        body: JSON.stringify({
          title: document.getElementById('event-title').value,
          region: document.getElementById('event-region').value,
          startDate: document.getElementById('event-start').value,
          endDate: document.getElementById('event-end').value
        })
      });
      showOutput(result);
      loadEvents();
    } catch (err) {
      showOutput({ error: err.message });
    }
  });
}
 
renderHeader();
renderCreateEventForm();
loadEvents();