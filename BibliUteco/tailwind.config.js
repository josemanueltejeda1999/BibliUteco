module.exports = {
  // Deshabilitar Preflight para evitar conflictos con Bootstrap
  corePlugins: {
    preflight: false,
  },
  // Agregar prefijo a las clases de Tailwind
  prefix: 'tw-',
  content: [
    "./**/*.razor",
    "./**/*.cshtml",
    "./**/*.html"
  ],
  theme: {
    extend: {
      colors: {
        bibliu: {
          DEFAULT: "#2E7D32"
        }
      }
    }
  },
  // Importante: marcar clases de Bootstrap como "safelist"
  safelist: [
    'row',
    'col',
    /^col-/,
    'container',
    'container-fluid',
    /^d-/,
    /^bg-/,
    /^text-/,
    /^m-/,
    /^p-/
  ]
};