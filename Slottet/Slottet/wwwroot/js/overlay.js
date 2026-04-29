window.overlayHelpers = {

    /**
     * Returnerer et elements bounding rect som et plain object.
     * Bruges til at måle kortets position FØR overlayets panel renderes.
     */
    getRect: function (elementId) {
        const el = document.getElementById(elementId);
        if (!el) return null;
        const r = el.getBoundingClientRect();
        return { x: r.x, y: r.y, width: r.width, height: r.height };
    },

    /**
     * FLIP-animation: panelet flyver fra kortets position til centrum.
     *
     * Teknik: panelet er allerede renderet centreret (top:50%, left:50%,
     * transform:translate(-50%,-50%)). Vi sætter CSS-variabler med offsettet
     * fra panelcentrum → kortcentrum, og scalen (kortets størrelse / panelets).
     * Derefter trigger vi CSS-animationen via en klasse.
     *
     * Alt sker via transform → GPU-accelereret, ingen layout-thrashing.
     */
    playFlyIn: function (panelId, cardRect) {
        const panel = document.getElementById(panelId);
        if (!panel) return;

        // Ingen kortRect = simpel fade+scale (bruges til sidebar-paneler)
        if (!cardRect) {
            panel.classList.remove('panel-simple-in');
            panel.offsetHeight;
            panel.classList.add('panel-simple-in');
            return;
        }

        // Mål panelet EFTER det er renderet (men stadig opacity:0)
        const panelRect = panel.getBoundingClientRect();

        // Centerpunkter
        const cardCX  = cardRect.x  + cardRect.width  / 2;
        const cardCY  = cardRect.y  + cardRect.height / 2;
        const panelCX = panelRect.x + panelRect.width  / 2;
        const panelCY = panelRect.y + panelRect.height / 2;

        // Offset fra panelets centrum → kortets centrum
        const tx = cardCX - panelCX;
        const ty = cardCY - panelCY;

        // Scale: kortets dimensioner relativt til panelets
        const sx = cardRect.width  / panelRect.width;
        const sy = cardRect.height / panelRect.height;

        // Sæt CSS custom properties på panelet
        panel.style.setProperty('--from-tx', tx + 'px');
        panel.style.setProperty('--from-ty', ty + 'px');
        panel.style.setProperty('--from-sx', sx);
        panel.style.setProperty('--from-sy', sy);

        // Trigger animationen (fjern og tilsæt klassen for at genstarte den)
        panel.classList.remove('panel-fly-in');
        panel.offsetHeight; // Tving reflow så animationen starter forfra
        panel.classList.add('panel-fly-in');
    }
};
