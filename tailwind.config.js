/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./**/*.{razor,html,cs}"],
  theme: {
    extend: {
      fontFamily: {
        sans: ["Roboto", "ui-sans-serif", "system-ui"],
      },
    },
  },
  plugins: [],
};
