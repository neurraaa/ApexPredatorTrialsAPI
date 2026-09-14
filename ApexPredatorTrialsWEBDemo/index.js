const output = document.getElementById('output');
function showOutput(data) { output.textContent = JSON.stringify(data, null, 2); }

function renderEvent(event) {
  const wrapper = document.createElement('div');
  wrapper.innerHTML = `
    <h3><a href="event.html?id=${event.id}">${event.title}</a> (${event.region})</h3>
    <p>${event.startDate} - ${event.endDate}</p>
    <p>Current round: <strong>${event.currentRound || event.startingRound || 'TBD'}</strong></p>
  `;

  if (isAdmin()) {
    const deleteBtn = document.createElement('button');
    deleteBtn.textContent = 'Delete event';
    deleteBtn.addEventListener('click', async () => {
      if (!confirm(`Delete event "${event.title}"?`)) return;
      try {
        await apiFetch(`/events/${event.id}`, { method: 'DELETE' });
        loadEvents();
      } catch (err) {
        showOutput({ error: err.message });
      }
    });
    wrapper.appendChild(deleteBtn);
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
      container.appendChild(renderEvent(event));
    }
  } catch (err) {
    container.textContent = `Failed to load events: ${err.message}`;
  }
}

const REGIONS = ['EU', 'NA', 'SA', 'AP'];
const PLATFORMS = ['Steam', 'Epic Games'];
const HUNTER_RANKS = ['Walker', 'Runner', 'Biter', 'Bolter', 'Stalker', 'Beast', 'Mauler', 'Juggernaut', 'Widow Maker', 'Carnivore', 'Hunter', 'Apex Predator'];
const HUMAN_RANKS = ['Prey', 'Casualty', 'Endangered', 'Underdog', 'Runner', 'Contender', 'Challenger', 'Fighter', 'Dominant', 'Ruthless', 'Indomitable', 'Ultimate Survivor'];
const STARTING_ROUND_MATCHUP_COUNT = { 'Quarter-Final': 4, 'Semi-Final': 2, 'Final': 1 };

function createPlayerEntryFields(role) {
  const wrapper = document.createElement('fieldset');
  wrapper.innerHTML = `
    <legend>${role}</legend>
    <div class="entry-search-row">
      <input type="text" class="entry-search" placeholder="Search existing player by name">
      <button type="button" class="entry-search-btn">Search</button>
      <button type="button" class="entry-clear-btn" style="display:none">Use a new player instead</button>
    </div>
    <ul class="entry-search-results"></ul>
    <p class="entry-selected" style="display:none"></p>
    <div class="entry-new-fields">
      <input type="text" class="entry-name" placeholder="Player name" required>
      <select class="entry-platform" required>
        ${PLATFORMS.map((p) => `<option value="${p}">${p}</option>`).join('')}
      </select>
      <select class="entry-region">
        ${REGIONS.map((r) => `<option value="${r}">${r}</option>`).join('')}
      </select>
    </div>
    <div class="entry-stats">
      <label>Hunter rank:
        <select class="stat-hunterRank" required>
          ${HUNTER_RANKS.map((r) => `<option value="${r}">${r}</option>`).join('')}
        </select>
      </label>
      <label>Hunter hours: <input type="number" class="stat-hunterHoursPlayed" value="0" min="0" required></label>
      <label>Mutation level: <input type="number" class="stat-mutationLevel" value="1" min="1" max="3" required></label>
      <label>Human rank:
        <select class="stat-humanRank" required>
          ${HUMAN_RANKS.map((r) => `<option value="${r}">${r}</option>`).join('')}
        </select>
      </label>
      <label>Human hours: <input type="number" class="stat-humanHoursPlayed" value="0" min="0" required></label>
      <label>Legend level: <input type="number" class="stat-legendLevel" value="1" min="1" max="250" required></label>
    </div>
  `;

  let selectedPlayerId = null;
  const searchInput = wrapper.querySelector('.entry-search');
  const searchBtn = wrapper.querySelector('.entry-search-btn');
  const clearBtn = wrapper.querySelector('.entry-clear-btn');
  const resultsList = wrapper.querySelector('.entry-search-results');
  const selectedLabel = wrapper.querySelector('.entry-selected');
  const newFields = wrapper.querySelector('.entry-new-fields');
  const nameInput = wrapper.querySelector('.entry-name');
  const platformInput = wrapper.querySelector('.entry-platform');

  async function runSearch() {
    const term = searchInput.value.trim();
    resultsList.innerHTML = '';
    if (!term) return;
    try {
      const matches = await apiFetch(`/players/search?name=${encodeURIComponent(term)}`);
      if (matches.length === 0) {
        resultsList.innerHTML = '<li>No matches.</li>';
        return;
      }
      matches.forEach((player) => {
        const li = document.createElement('li');
        const btn = document.createElement('button');
        btn.type = 'button';
        btn.textContent = `${player.name} (${player.platform}, ${player.region})`;
        btn.addEventListener('click', () => selectExisting(player));
        li.appendChild(btn);
        resultsList.appendChild(li);
      });
    } catch (err) {
      resultsList.innerHTML = `<li>Search failed: ${err.message}</li>`;
    }
  }

  async function selectExisting(player) {
    selectedPlayerId = player.id;
    selectedLabel.style.display = '';
    selectedLabel.textContent = `Updating stats for existing player: ${player.name}`;
    newFields.style.display = 'none';
    // Fields inside a hidden container are still subject to native form validation
    // (and can't be focused to show the validation error), so drop `required` while hidden.
    nameInput.required = false;
    platformInput.required = false;
    resultsList.innerHTML = '';
    searchInput.style.display = 'none';
    searchBtn.style.display = 'none';
    clearBtn.style.display = '';

    try {
      const profile = await apiFetch(`/players/${player.id}/profile`);
      if (profile.stats) {
        wrapper.querySelector('.stat-hunterRank').value = profile.stats.hunterRank;
        wrapper.querySelector('.stat-hunterHoursPlayed').value = profile.stats.hunterHoursPlayed;
        wrapper.querySelector('.stat-mutationLevel').value = profile.stats.mutationLevel;
        wrapper.querySelector('.stat-humanRank').value = profile.stats.humanRank;
        wrapper.querySelector('.stat-humanHoursPlayed').value = profile.stats.humanHoursPlayed;
        wrapper.querySelector('.stat-legendLevel').value = profile.stats.legendLevel;
      }
    } catch (err) {
      showOutput({ error: err.message });
    }
  }

  function clearSelection() {
    selectedPlayerId = null;
    selectedLabel.style.display = 'none';
    newFields.style.display = '';
    nameInput.required = true;
    platformInput.required = true;
    searchInput.style.display = '';
    searchBtn.style.display = '';
    clearBtn.style.display = 'none';
    searchInput.value = '';
  }

  searchBtn.addEventListener('click', runSearch);
  clearBtn.addEventListener('click', clearSelection);

  function getEntry() {
    const stats = {
      hunterRank: wrapper.querySelector('.stat-hunterRank').value,
      hunterHoursPlayed: Number(wrapper.querySelector('.stat-hunterHoursPlayed').value),
      mutationLevel: Number(wrapper.querySelector('.stat-mutationLevel').value),
      humanRank: wrapper.querySelector('.stat-humanRank').value,
      humanHoursPlayed: Number(wrapper.querySelector('.stat-humanHoursPlayed').value),
      legendLevel: Number(wrapper.querySelector('.stat-legendLevel').value)
    };

    if (selectedPlayerId) return { playerId: selectedPlayerId, stats };

    return {
      name: nameInput.value,
      platform: platformInput.value,
      region: wrapper.querySelector('.entry-region').value,
      stats
    };
  }

  return { element: wrapper, getEntry };
}

function createMatchupFields(index) {
  const fieldset = document.createElement('fieldset');
  fieldset.innerHTML = `<legend>Matchup ${index}</legend>`;
  const hunter = createPlayerEntryFields('Hunter');
  const human = createPlayerEntryFields('Human');
  fieldset.appendChild(hunter.element);
  fieldset.appendChild(human.element);
  return {
    element: fieldset,
    getMatchup: () => ({ hunter: hunter.getEntry(), human: human.getEntry() })
  };
}

function renderCreateEventForm() {
  const placeholder = document.getElementById('create-event-placeholder');
  if (!getUser()) {
    placeholder.innerHTML = '';
    return;
  }

  placeholder.innerHTML = `
    <h2>Create Event</h2>
    <form id="create-event-form">
      <input type="text" id="event-title" placeholder="Title" required>
      <select id="event-region">
        ${REGIONS.map((r) => `<option value="${r}">${r}</option>`).join('')}
      </select>
      <label>Start: <input type="date" id="event-start" required></label>
      <label>End: <input type="date" id="event-end" required></label>
      <label>Starting round:
        <select id="event-starting-round">
          <option value="Quarter-Final">Quarter-Final</option>
          <option value="Semi-Final">Semi-Final</option>
        </select>
      </label>
      <h3>Matchups</h3>
      <p id="matchups-hint"></p>
      <div id="matchups-container"></div>
      <button type="submit">Create Event</button>
    </form>
  `;

  const matchupsContainer = document.getElementById('matchups-container');
  const matchupsHint = document.getElementById('matchups-hint');
  const startingRoundSelect = document.getElementById('event-starting-round');
  let matchups = [];

  function renderMatchupsForRound() {
    const requiredCount = STARTING_ROUND_MATCHUP_COUNT[startingRoundSelect.value];
    matchupsHint.textContent = `${startingRoundSelect.value} requires exactly ${requiredCount} matchups. Search for an existing player to update their stats, or leave the search empty and fill in a brand-new player.`;

    matchupsContainer.innerHTML = '';
    matchups = [];
    for (let i = 0; i < requiredCount; i++) {
      const matchup = createMatchupFields(i + 1);
      matchups.push(matchup);
      matchupsContainer.appendChild(matchup.element);
    }
  }

  startingRoundSelect.addEventListener('change', renderMatchupsForRound);
  renderMatchupsForRound();

  document.getElementById('create-event-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    try {
      const result = await apiFetch('/events', {
        method: 'POST',
        body: JSON.stringify({
          title: document.getElementById('event-title').value,
          region: document.getElementById('event-region').value,
          startDate: document.getElementById('event-start').value,
          endDate: document.getElementById('event-end').value,
          startingRound: document.getElementById('event-starting-round').value,
          matchups: matchups.map((m) => m.getMatchup())
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
