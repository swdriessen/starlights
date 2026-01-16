import { ComponentExample } from "@/components/component-example";
import { create_api_client } from "@starlights/content-api";

export function App() {
  create_api_client("http://localhost:3000");

  return <ComponentExample />;
}

export default App;
