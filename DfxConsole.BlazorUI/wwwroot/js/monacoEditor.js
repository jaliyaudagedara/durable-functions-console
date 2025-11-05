let monacoLoaded = false;
let monacoLoadPromise = null;

// Wait for Monaco CSS to be fully loaded by checking for Monaco-specific styles
function waitForMonacoStyles(timeout = 10000) {
    return new Promise((resolve, reject) => {
        const startTime = Date.now();

        function checkStyles() {
            // More comprehensive CSS check - look for multiple Monaco style indicators
            const checks = [];

            // Check 1: Monaco editor styles
            const editorDiv = document.createElement('div');
            editorDiv.className = 'monaco-editor';
            editorDiv.style.visibility = 'hidden';
            editorDiv.style.position = 'absolute';
            editorDiv.style.left = '-9999px';
            document.body.appendChild(editorDiv);

            const editorStyles = window.getComputedStyle(editorDiv);
            checks.push(editorStyles.position === 'relative' || editorStyles.overflow === 'hidden');
            document.body.removeChild(editorDiv);

            // Check 2: Look for Monaco CSS link or style tag
            const hasMonacoStylesheet = Array.from(document.styleSheets).some(sheet => {
                try {
                    return sheet.href && sheet.href.includes('monaco-editor');
                } catch (e) {
                    return false;
                }
            });

            // Check 3: Check if Monaco theme classes exist
            const hasMonacoTheme = Array.from(document.styleSheets).some(sheet => {
                try {
                    if (!sheet.cssRules) return false;
                    return Array.from(sheet.cssRules).some(rule =>
                        rule.selectorText && (
                            rule.selectorText.includes('.vs-dark') ||
                            rule.selectorText.includes('.monaco-editor')
                        )
                    );
                } catch (e) {
                    return false;
                }
            });

            const isStylesLoaded = checks.some(c => c) || hasMonacoStylesheet || hasMonacoTheme;

            if (isStylesLoaded) {
                console.log('Monaco CSS detected and loaded');
                resolve();
            } else if (Date.now() - startTime > timeout) {
                console.warn('Monaco CSS not fully detected after timeout, attempting to force load');
                // Force reload Monaco CSS by clearing and reloading
                forceLoadMonacoCSS().then(() => {
                    console.log('Monaco CSS force loaded');
                    resolve();
                }).catch(() => {
                    console.error('Failed to force load Monaco CSS');
                    resolve(); // Still proceed
                });
            } else {
                setTimeout(checkStyles, 100);
            }
        }

        checkStyles();
    });
}

// Force load Monaco CSS if it's missing
function forceLoadMonacoCSS() {
    return new Promise((resolve, reject) => {
        // Check if CSS link already exists
        const existingLink = document.querySelector('link[href*="monaco-editor"][href*="editor.main.css"]');
        if (existingLink) {
            resolve();
            return;
        }

        // Add Monaco CSS link manually
        const cssLink = document.createElement('link');
        cssLink.rel = 'stylesheet';
        cssLink.href = 'https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.45.0/min/vs/editor/editor.main.css';
        cssLink.onload = () => {
            console.log('Monaco CSS manually loaded');
            setTimeout(resolve, 100); // Give it a moment to apply
        };
        cssLink.onerror = () => {
            console.error('Failed to load Monaco CSS manually');
            reject();
        };
        document.head.appendChild(cssLink);
    });
}

// Preload Monaco Editor
function loadMonaco() {
    if (monacoLoadPromise) return monacoLoadPromise;

    if (window.monaco) {
        monacoLoaded = true;
        // Even if Monaco is loaded, ensure CSS is ready
        return waitForMonacoStyles();
    }

    monacoLoadPromise = new Promise((resolve, reject) => {
        const loaderScript = document.createElement('script');
        loaderScript.src = 'https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.45.0/min/vs/loader.min.js';
        loaderScript.onload = () => {
            require.config({ paths: { 'vs': 'https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.45.0/min/vs' } });
            require(['vs/editor/editor.main'], async () => {
                // Wait for CSS to be fully loaded
                await waitForMonacoStyles();
                monacoLoaded = true;
                resolve();
            });
        };
        loaderScript.onerror = () => reject(new Error('Failed to load Monaco Editor'));
        document.head.appendChild(loaderScript);
    });

    return monacoLoadPromise;
}

// Start loading Monaco immediately when script loads
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', loadMonaco);
} else {
    loadMonaco();
}

export async function initializeMonacoEditor(container, content, retryCount = 0) {
    const MAX_RETRIES = 2;

    try {
        // Wait for Monaco to be ready (should already be loaded)
        await loadMonaco();

        // Double check that Monaco is actually available
        if (!window.monaco) {
            throw new Error('Monaco not available after loading');
        }

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
            contextmenu: true,
            // Enable context menu related features for better UX
            quickSuggestions: false,
            parameterHints: { enabled: false },
            suggestOnTriggerCharacters: false,
            acceptSuggestionOnEnter: 'off',
            tabCompletion: 'off',
            wordBasedSuggestions: false,
            fixedOverflowWidgets: false
        });

        container._editor = editor;

        // Verify the editor was created successfully
        await new Promise((resolve) => setTimeout(resolve, 100));

        if (!container._editor) {
            throw new Error('Editor not attached to container');
        }

        // Verify Monaco CSS is actually applied to the editor
        const editorElement = container.querySelector('.monaco-editor');
        if (editorElement) {
            const styles = window.getComputedStyle(editorElement);
            const hasStyles = styles.position === 'relative' || styles.overflow === 'hidden';

            if (!hasStyles) {
                console.warn('Monaco editor rendered but CSS not applied! Attempting to fix...');
                // Try to force load CSS
                await forceLoadMonacoCSS();

                // Give CSS time to apply
                await new Promise((resolve) => setTimeout(resolve, 500));

                // Check again
                const updatedStyles = window.getComputedStyle(editorElement);
                const nowHasStyles = updatedStyles.position === 'relative' || updatedStyles.overflow === 'hidden';

                if (!nowHasStyles) {
                    console.error('Monaco CSS still not applied after force load. Editor may not display correctly.');
                    // Add a visual indicator
                    container.style.border = '2px solid #ff6b6b';
                    container.title = 'Monaco editor styles failed to load. Try refreshing the page.';
                } else {
                    console.log('Monaco CSS successfully applied after force load');
                }
            }
        }

        try {
            editor.getAction('editor.action.formatDocument')?.run();
        } catch (e) {
            console.warn('Format document failed:', e);
        }

    } catch (error) {
        console.error('Error initializing Monaco editor:', error);

        if (retryCount < MAX_RETRIES) {
            console.log(`Retrying Monaco initialization (${retryCount + 1}/${MAX_RETRIES})...`);
            await new Promise(resolve => setTimeout(resolve, 500));
            return initializeMonacoEditor(container, content, retryCount + 1);
        } else {
            // Final fallback: show error in container
            container.innerHTML = `
                <div style="padding: 20px; color: #ff6b6b; background: #2d2d2d; font-family: monospace;">
                    <strong>Monaco Editor failed to load.</strong><br>
                    <small>Refresh the page to try again.</small>
                </div>`;
            throw error;
        }
    }
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

export function resizeEditor(container) {
    if (container._editor) {
        try {
            container._editor.layout();
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