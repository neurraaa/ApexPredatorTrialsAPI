function getMatchIdFromUrl() {
  const params = new URLSearchParams(window.location.search);
  return params.get('id');
}

async function loadMatch() {
  const container = document.getElementById('match-container');
  const id = getMatchIdFromUrl();

  if (!id) {
    container.textContent = 'No match id given. Open this page as match.html?id=1';
    return;
  }

  try {
    const match = await apiFetch(`/matches/${id}`);

    let resultHtml = '<p>No results recorded yet.</p>';
    try {
      const result = await apiFetch(`/match-results/by-match/${id}`);
      resultHtml = `
        <p><strong>Winner:</strong> ${playerLink(result.winnerId, result.winnerName)}</p>
        <p><strong>Loser:</strong> ${playerLink(result.loserId, result.loserName)}</p>
        <p><strong>Winner deaths:</strong> ${result.winnerDeaths} &nbsp; <strong>Winner kills:</strong> ${result.winnerKills}</p>
        <p><strong>Loser deaths:</strong> ${result.loserDeaths} &nbsp; <strong>Loser kills:</strong> ${result.loserKills}</p>
        <p><strong>Nests destroyed:</strong> ${result.nestsDestroyed}</p>
      `;
    } catch {
      // No result recorded yet — keep the default message.
    }

    container.innerHTML = `
      <p><strong>Hunter:</strong> ${playerLink(match.hunterPlayerId, match.hunterPlayerName)}</p>
      <p><strong>Human:</strong> ${playerLink(match.humanPlayerId, match.humanPlayerName)}</p>
      <p><strong>Map:</strong> ${match.mapName}</p>
      <p><strong>Region:</strong> ${match.region}</p>
      <p><strong>Date:</strong> ${match.dateTime}</p>
      <h2>Result</h2>
      ${resultHtml}
    `;
  } catch (err) {
    container.textContent = `Failed to load match: ${err.message}`;
  }
}

renderHeader();
loadMatch();
