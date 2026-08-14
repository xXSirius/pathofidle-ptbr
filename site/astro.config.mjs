import { defineConfig } from 'astro/config';
import react from '@astrojs/react';
import sitemap from '@astrojs/sitemap';
import icon from 'astro-icon';
import tailwindcss from '@tailwindcss/vite';

// Tailwind v4: configurado via plugin do Vite (CSS-first).
// Não existe mais tailwind.config.js — os tokens vivem em src/styles/global.css (@theme).

export default defineConfig({
  output: 'static',
  // GitHub Pages (projeto, não domínio custom): serve em /pathofidle-ptbr/.
  // Se um domínio próprio for configurado depois, troque `site` e zere `base`.
  site: 'https://xxsirius.github.io/',
  base: '/pathofidle-ptbr/',
  // Build sai direto na pasta que o GitHub Pages serve (docs/ na raiz do repo,
  // configurado em Settings → Pages → Deploy from branch → main → /docs).
  outDir: '../docs',
  integrations: [
    react(),
    sitemap(),
    icon(),
  ],
  compressHTML: true,
  build: {
    inlineStylesheets: 'auto',
  },
  vite: {
    plugins: [tailwindcss()],
    // Tailwind v4 dispensa PostCSS. Config inline vazio impede o Vite de
    // subir a árvore e herdar um postcss.config de um projeto-pai (ex: o studio).
    css: {
      postcss: {},
    },
  },
});
