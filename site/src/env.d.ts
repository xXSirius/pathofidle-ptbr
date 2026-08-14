/// <reference types="astro/client" />

interface ImportMetaEnv {
  readonly BASE_URL: string
  readonly PUBLIC_GTM_ID: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
