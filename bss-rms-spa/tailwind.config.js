/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
    "./node_modules/flowbite/**/*.js"
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          400: '#66bb6a',
          600: '#43a047',
          700: '#388e3c',
        },
        secondary: {
          light: '#E8ECE7',
          'light-100': '#C8E6C9',
        },
        ternary: {
          light: '#F1FAF5',
        },
        warning: {
          50: '#ffebee',
          100: '#ffcdd2',
          200: '#ef9a9a',
          300: '#e57373',
          400: '#ef5350',
          600: '#e53935',
          700: '#ff1744',
        },
        text: {
          primary: 'white',
          'dark-primary': '#424242',
          'dark-secondary': '#616161',
        },
      },
      spacing: {
        'xs': '4px',
        'sm': '8px',
        'md': '16px',
        'lg': '24px',
        'xl': '32px',
        'xxl': '48px',
      },
      borderRadius: {
        'sm': '4px',
        'md': '8px',
        'lg': '12px',
        'xl': '16px',
      },
      fontFamily: {
        'overpass': ['Overpass', 'sans-serif'],
      },
    },
  },
  plugins: [
    require('flowbite/plugin')
  ],
}