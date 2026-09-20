const base = document.baseURI;
const path = window.location.pathname;
if (path.endsWith('/manifest') || path.endsWith('/cards')) {
    const id = new URLSearchParams(window.location.search).get('id');
    if (id) {
        fetch(new URL('data/themes.json', base)).then(response => response.json()).then(index => {
            const theme = index.themes.find(item => item.id.toLowerCase() === id.toLowerCase() || item.name.toLowerCase() === id.toLowerCase());
            if (theme) window.location.replace(new URL(`data/${theme.id}.${path.endsWith('/manifest') ? 'manifest' : 'cards'}.json`, base));
        });
    }
}
