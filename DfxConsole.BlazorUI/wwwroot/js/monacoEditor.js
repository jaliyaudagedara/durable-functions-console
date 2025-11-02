let monacoLoaded = false;

async function loadMonaco() {
    if (monacoLoaded) return;
    if (window.monaco) {
        monacoLoaded = true;
        return;
    }

    return new Promise((resolve, reject) => {
        const loaderScript = document.createElement('script');
        loaderScript.src = 'https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.45.0/min/vs/loader.min.js';
        loaderScript.onload = () => {
            require.config({ paths: { 'vs': 'https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.45.0/min/vs' } });
            require(['vs/editor/editor.main'], () => {
                monacoLoaded = true;
                resolve();
            });
        };
        loaderScript.onerror = () => reject(new Error('Failed to load Monaco Editor'));
        document.head.appendChild(loaderScript);
    });
}

export async function initializeMonacoEditor(container, content) {
    await loadMonaco();
    
    container.innerHTML = '';
    
    const editor = monaco.editor.create(container, {
        value: content || '',
        language: 'json',
        theme: 'vs-dark',
        automaticLayout: true,
        readOnly: true,
        minimap: { enabled: false },
        scrollBeyondLastLine: false,
        fontSize: 14,
        lineNumbers: 'on',
        wordWrap: 'on',
        contextmenu: false,
        // CRITICAL: Disable all context menu related features
        quickSuggestions: false,
        parameterHints: { enabled: false },
        suggestOnTriggerCharacters: false,
        acceptSuggestionOnEnter: 'off',
        tabCompletion: 'off',
        wordBasedSuggestions: false,
        // Prevent Monaco from trying to attach context menu handlers
        fixedOverflowWidgets: true
    });

    container._editor = editor;

    // Prevent right-click entirely at the DOM level
    container.addEventListener('contextmenu', (e) => {
        e.preventDefault();
        e.stopPropagation();
        return false;
    }, true);

    setTimeout(() => {
        try {
            editor.getAction('editor.action.formatDocument')?.run();
        } catch (e) {}
    }, 100);
}

export function updateEditorContent(container, content) {
    if (container._editor) {
        try {
            container._editor.setValue(content || '');
            setTimeout(() => {
                try {
                    container._editor.getAction('editor.action.formatDocument')?.run();
                } catch (e) {}
            }, 100);
        } catch (e) {}
    }
}

export function disposeEditorSync(container) {
    if (container._editor) {
        try {
            container._editor.dispose();
            delete container._editor;
        } catch (e) {}
    }
}