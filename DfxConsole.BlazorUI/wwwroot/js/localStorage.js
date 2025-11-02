window.localStorageInterop = {
    setItem: function (key, value) {
        try {
            localStorage.setItem(key, value);
            return true;
        } catch (err) {
            console.error('Failed to set item in localStorage: ', err);
            return false;
        }
    },

    getItem: function (key) {
        try {
            return localStorage.getItem(key);
        } catch (err) {
            console.error('Failed to get item from localStorage: ', err);
            return null;
        }
    },

    removeItem: function (key) {
        try {
            localStorage.removeItem(key);
            return true;
        } catch (err) {
            console.error('Failed to remove item from localStorage: ', err);
            return false;
        }
    },

    clear: function () {
        try {
            localStorage.clear();
            return true;
        } catch (err) {
            console.error('Failed to clear localStorage: ', err);
            return false;
        }
    }
};