// VentouxTina Map interop — Leaflet integration
// Requires Leaflet CSS and JS loaded via CDN in App.razor <head>

let _map = null;
let _routeLayer = null;
let _progressLayer = null;

window.ventouxMap = {
    init: function (elementId, routeCoords, progressCoords) {
        if (_map) {
            _map.remove();
            _map = null;
        }

        _map = L.map(elementId, {zoomControl: true});

        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
            maxZoom: 18,
            attribution: "&copy; <a href='https://www.openstreetmap.org/copyright'>OpenStreetMap</a>-bijdragers",
        }).addTo(_map);

        // Full route — grey dashed
        if (routeCoords && routeCoords.length > 1) {
            _routeLayer = L.polyline(routeCoords, {
                color: "#9ca3af",
                weight: 3,
                dashArray: "6, 4",
                opacity: 0.8,
            }).addTo(_map);
        }

        // Traveled segment — brand color
        if (progressCoords && progressCoords.length > 1) {
            _progressLayer = L.polyline(progressCoords, {
                color: "#2563eb",
                weight: 5,
                opacity: 0.95,
            }).addTo(_map);
        }

        // Fit to full route; fall back to progress if route not provided
        const fitTarget = _routeLayer || _progressLayer;
        if (fitTarget) {
            _map.fitBounds(fitTarget.getBounds(), {padding: [20, 20]});
        }
    },

    updateProgress: function (progressCoords) {
        if (!_map) return;
        if (_progressLayer) {
            _map.removeLayer(_progressLayer);
        }
        if (progressCoords && progressCoords.length > 1) {
            _progressLayer = L.polyline(progressCoords, {
                color: "#2563eb",
                weight: 5,
                opacity: 0.95,
            }).addTo(_map);
        }
    },
};
