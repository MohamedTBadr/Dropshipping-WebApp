// Utility helper for API requests
// Adds Authorization header automatically if a token is stored in localStorage
function getAuthHeaders(extra = {}) {
    const token = localStorage.getItem('token');
    const headers = Object.assign({}, extra);
    if (token) headers['Authorization'] = `Bearer ${token}`;
    return headers;
}

// keep backwards-compatible: expose on window in case scripts expect global function
window.getAuthHeaders = getAuthHeaders;
