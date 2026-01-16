// src/index.ts
function test_from_content_api_client() {
  console.log("Content API Client Loaded");
  return true;
}
function create_api_client(endpoint_base) {
  return "API client for " + endpoint_base;
}
export {
  create_api_client,
  test_from_content_api_client
};
