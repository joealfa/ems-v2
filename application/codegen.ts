import type { CodegenConfig } from "@graphql-codegen/cli";

const config: CodegenConfig = {
  // Your GraphQL Gateway endpoint
  schema: "https://localhost:5003/graphql",
  // Where to find your GraphQL operations (queries, mutations, fragments)
  documents: ["src/**/*.graphql", "src/**/operations.ts"],
  // Ignore generated files
  ignoreNoDocuments: true,
  generates: {
    // Output directory for generated types and hooks
    "./src/graphql/generated/": {
      preset: "client",
      config: {
        // Use TypeScript enums
        enumsAsTypes: true,
        // Skip __typename in types
        skipTypename: false,
        // Use 'import type' for type imports
        useTypeImports: true,
        // Avoid 'as const' assertions for better compatibility
        arrayInputCoercion: false,
        // Make all fields optional by default (safer for partial responses)
        avoidOptionals: false,
        // Scalars mapping
        scalars: {
          DateTime: "string",
          Date: "string",
          Decimal: "number",
          Long: "number",
          UUID: "string",
          Upload: "File",
        },
      },
      presetConfig: {
        // Generate fragment masking for better type safety
        fragmentMasking: false,
      },
    },
  },
  hooks: {
    afterAllFileWrite: ["prettier --write"],
  },
};

export default config;
