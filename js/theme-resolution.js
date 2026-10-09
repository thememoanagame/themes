(() => {
    const resolver = document.body.dataset.resolver;
    const dataRoot = document.body.dataset.dataRoot;

    if (!resolver || !dataRoot) {
        return;
    }

    const query = new URLSearchParams(window.location.search);
    const id = query.get("id")?.trim() ?? "";
    const name = query.get("name")?.trim() ?? "";

    const redirect = relativePath => {
        window.location.replace(new URL(relativePath, document.baseURI).href);
    };

    const redirectError = file => redirect(dataRoot + "errors/" + file);

    if (resolver === "themes" && !id && !name) {
        redirect(dataRoot + "version.json");
        return;
    }

    if (!id && !name) {
        redirectError("invalid-query.json");
        return;
    }

    if (id && !/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(id)) {
        redirectError("invalid-query.json");
        return;
    }

    const catalogUrl = new URL(dataRoot + "themes.json", document.baseURI);

    fetch(catalogUrl, { cache: "force-cache" })
        .then(response => {
            if (!response.ok) {
                throw new Error("Theme catalog request failed.");
            }

            return response.json();
        })
        .then(catalog => {
            if (!Array.isArray(catalog.themes)) {
                throw new Error("Theme catalog is invalid.");
            }

            const byId = id
                ? catalog.themes.find(theme => typeof theme.id === "string" && theme.id.toLowerCase() === id.toLowerCase())
                : null;

            const byName = name
                ? catalog.themes.find(theme => typeof theme.name === "string" && theme.name.toLowerCase() === name.toLowerCase())
                : null;

            if ((id && !byId) || (name && !byName)) {
                redirectError("not-found.json");
                return;
            }

            if (byId && byName && byId.id.toLowerCase() !== byName.id.toLowerCase()) {
                redirectError("invalid-query.json");
                return;
            }

            const theme = byId ?? byName;
            const resource = resolver === "themes"
                ? theme.id + "/manifest.json"
                : theme.id + "/cards.json";

            redirect(dataRoot + resource);
        })
        .catch(() => redirectError("invalid-theme.json"));
})();
