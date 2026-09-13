function getPlayerIdFromUrl() {
  const params = new URLSearchParams(window.location.search);
  return params.get('id');
}
 
async function loadPlayer() {
  const container = document.getElementById('player-container');
  const id = getPlayerIdFromUrl();
 
  if (!id) {
    container.textContent = 'No player id given. Open this page as player.html?id=1';
    return;
  }
 
  try {
    const player = await apiFetch(`/players/${id}/profile`);
    const stats = player.stats;
 
    container.innerHTML = `
      <p><strong>Name:</strong> ${player.name}</p>
      <p><strong>Platform:</strong> ${player.platform}</p>
      <p><strong>Region:</strong> ${player.region}</p>
      <p><strong>Linked account:</strong> ${player.username ?? 'No account registered'}</p>
      <h2>Stats</h2>
      ${stats ? `
        <p><strong>Hunter rank:</strong> ${stats.hunterRank} (${stats.hunterHoursPlayed}h, Mutation Level ${stats.mutationLevel})</p>
        <p><strong>Human rank:</strong> ${stats.humanRank} (${stats.humanHoursPlayed}h, Legend Level ${stats.legendLevel})</p>
        <p><strong>Total hours:</strong> ${stats.hoursTotal}</p>
      ` : '<p>No stats recorded.</p>'}
    `;
  } catch (err) {
    container.textContent = `Failed to load player: ${err.message}`;
  }
}
 
renderHeader();
loadPlayer();