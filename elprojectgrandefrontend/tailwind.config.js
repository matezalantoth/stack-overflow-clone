export default {
    content: [
        "./index.html",
        "./src/**/*.{js,ts,jsx,tsx}",
    ],
    theme: {
        fontFamily: {
            'lato': ['Lato'],
        }, extend: {
            width: {
                '120': '30rem',
            },
            animation: {
                border: 'border 4s ease infinite',
            },
            keyframes: {
                border: {
                    '0%, 100%': {backgroundPosition: '0% 50%'},
                    '50%': {backgroundPosition: '100% 50%'},
                },
            },
            transform: {
                'preserve-3d': 'preserve-3d',
            },
            rotate: {
                'x-180': 'rotateX(180deg)',
            },
            perspective: {
                '1000': '1000px',
            },
            transitionDuration: {
                '1000': '1000ms',
            },
        }
    },
    plugins: [],
}