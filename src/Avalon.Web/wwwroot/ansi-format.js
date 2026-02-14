// This file provides a JavaScript function to parse ANSI color codes and convert them to HTML spans with CSS classes for color and style, mimicking AvalonMudClient formatting.

// Basic ANSI color code to CSS class mapping (extend as needed)
function ansiToHtml(text) {
    // Replace ANSI escape codes with HTML spans
    let html = '';
    let openTags = [];
    let regex = /(\x1B\[[0-9;]*m)/g;
    let lastIndex = 0;
    let match;
    let currentColor = null;
    let currentStyles = new Set();
    while ((match = regex.exec(text)) !== null) {
        // Output text before this escape code
        html += escapeHtml(text.substring(lastIndex, match.index));
        const codes = match[0].slice(2, -1).split(';');
        let reset = codes.includes('0');
        let newColor = null;
        let newStyles = new Set();
        for (const code of codes) {
            if (ANSI_COLORS[code]) newColor = code;
            if (ANSI_STYLES[code] && code !== '0') newStyles.add(code);
        }
        // If reset, close all tags, but if a color follows immediately, delay closing
        if (reset && newColor) {
            // If the only codes are 0 and a color, treat as a color change, not a full reset
            if (currentColor !== newColor) {
                if (currentColor) html += openTags.pop();
                html += `<span class="${ANSI_COLORS[newColor]}">`;
                openTags.push('</span>');
                currentColor = newColor;
            }
            // Styles still reset
            while (currentStyles.size) { html += openTags.pop(); currentStyles.clear(); }
        } else {
            if (reset) {
                while (openTags.length) html += openTags.pop();
                currentColor = null;
                currentStyles.clear();
            }
            // Only open a new color span if the color actually changes
            if (newColor && newColor !== currentColor) {
                if (currentColor) html += openTags.pop();
                html += `<span class="${ANSI_COLORS[newColor]}">`;
                openTags.push('</span>');
                currentColor = newColor;
            }
            // Handle styles (bold, underline, etc.)
            for (const style of newStyles) {
                if (!currentStyles.has(style)) {
                    html += `<span class="${ANSI_STYLES[style]}">`;
                    openTags.push('</span>');
                    currentStyles.add(style);
                }
            }
            // Remove styles that are no longer present
            for (const style of Array.from(currentStyles)) {
                if (!newStyles.has(style)) {
                    html += openTags.pop();
                    currentStyles.delete(style);
                }
            }
        }
        lastIndex = regex.lastIndex;
    }
    // Output any remaining text after the last escape code
    html += escapeHtml(text.substring(lastIndex));
    // Remove any stray visible escape sequences (e.g. if not matched by regex)
    html = html.replace(/\x1B\[[0-9;]*m/g, '');
    while (openTags.length) html += openTags.pop();
    return html;
}
            if (newColor && newColor !== currentColor) {
                if (currentColor) html += openTags.pop();
                html += `<span class="${ANSI_COLORS[newColor]}">`;
                openTags.push('</span>');
                currentColor = newColor;
            }
            // Handle styles (bold, underline, etc.)
            for (const style of newStyles) {
                if (!currentStyles.has(style)) {
                    html += `<span class="${ANSI_STYLES[style]}">`;
                    openTags.push('</span>');
                    currentStyles.add(style);
                }
            }
            // Remove styles that are no longer present
            for (const style of Array.from(currentStyles)) {
                if (!newStyles.has(style)) {
                    html += openTags.pop();
                    currentStyles.delete(style);
                }
            }
        }
        lastIndex = regex.lastIndex;
    }
    html += escapeHtml(text.substring(lastIndex));
    while (openTags.length) html += openTags.pop();
    return html;
}

function escapeHtml(str) {
    return str.replace(/[&<>]/g, tag => ({'&':'&amp;','<':'&lt;','>':'&gt;'}[tag]));
}

// Export for use in browser
window.ansiToHtml = ansiToHtml;
