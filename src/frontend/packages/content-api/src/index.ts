export function test_from_content_api_client() {
  console.log("Content API Client Loaded");
  return true;
}

export function create_api_client(endpoint_base: string) {
  return "API client for " + endpoint_base;
}
