// Wait for Flatpickr to be available (loaded via App.razor)
function waitForFlatpickr() {
    return new Promise((resolve, reject) => {
        if (window.flatpickr) {
            resolve();
            return;
        }

        let attempts = 0;
        const maxAttempts = 50; // 5 seconds max wait

        const checkInterval = setInterval(() => {
            attempts++;
            if (window.flatpickr) {
                clearInterval(checkInterval);
                resolve();
            } else if (attempts >= maxAttempts) {
                clearInterval(checkInterval);
                reject(new Error('Flatpickr library not loaded'));
            }
        }, 100);
    });
}

export async function initializeDateTimePicker(input, dotNetRef, enableTime, initialValue) {
    // Wait for Flatpickr to be available
    await waitForFlatpickr();

    if (!input) return;

    // Destroy existing instance if any
    if (input._flatpickr) {
        input._flatpickr.destroy();
    }

    const options = {
        enableTime: enableTime,
        dateFormat: enableTime ? "Y-m-d H:i" : "Y-m-d",
        time_24hr: true,
        allowInput: true,
        clickOpens: true,
        onChange: function (selectedDates, dateStr, instance) {
            if (selectedDates.length > 0) {
                // Format as local datetime string (not UTC) to match original datetime-local behavior
                const date = selectedDates[0];
                const year = date.getFullYear();
                const month = String(date.getMonth() + 1).padStart(2, '0');
                const day = String(date.getDate()).padStart(2, '0');
                const hours = String(date.getHours()).padStart(2, '0');
                const minutes = String(date.getMinutes()).padStart(2, '0');
                const seconds = String(date.getSeconds()).padStart(2, '0');

                // Format: "YYYY-MM-DDTHH:mm:ss" (local time, not UTC)
                const localDateTimeString = `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;
                dotNetRef.invokeMethodAsync('OnDateChanged', localDateTimeString);
            } else {
                dotNetRef.invokeMethodAsync('OnDateChanged', null);
            }
        },
        onClose: function (selectedDates, dateStr, instance) {
            // Blur the input to remove focus
            input.blur();
        }
    };

    // Set initial value if provided
    if (initialValue) {
        options.defaultDate = new Date(initialValue);
    }

    const fp = flatpickr(input, options);
    input._flatpickr = fp;

    // Make input clickable
    input.addEventListener('click', () => {
        if (fp) fp.open();
    });
}

export function clearDateTimePicker(input) {
    if (input && input._flatpickr) {
        input._flatpickr.clear();
    }
}

export function destroyDateTimePicker(input) {
    if (input && input._flatpickr) {
        input._flatpickr.destroy();
        delete input._flatpickr;
    }
}
