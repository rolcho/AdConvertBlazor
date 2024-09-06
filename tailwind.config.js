/** @type {import('tailwindcss').Config} */
/* eslint-disable */
/* prettier-ignore */
module.exports = {
  content: ["./**/*.{razor,html,cs}"],
  theme: {
    extend: {
      fontFamily: {
        sans: ["Roboto", "ui-sans-serif", "system-ui"],
      },
    },
    borderRadius: {
      'none': '0',
      'sm': '0.125rem',
      DEFAULT: '4px',
      'md': '0.375rem',
      'lg': '0.5rem',
      'xl': '0.75rem',
      'full': '9999px',
      'large': '40px',
    },
  },
  plugins: [],
};
