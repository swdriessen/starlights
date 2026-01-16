"use strict";
var __defProp = Object.defineProperty;
var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
var __getOwnPropNames = Object.getOwnPropertyNames;
var __hasOwnProp = Object.prototype.hasOwnProperty;
var __export = (target, all) => {
  for (var name in all)
    __defProp(target, name, { get: all[name], enumerable: true });
};
var __copyProps = (to, from, except, desc) => {
  if (from && typeof from === "object" || typeof from === "function") {
    for (let key of __getOwnPropNames(from))
      if (!__hasOwnProp.call(to, key) && key !== except)
        __defProp(to, key, { get: () => from[key], enumerable: !(desc = __getOwnPropDesc(from, key)) || desc.enumerable });
  }
  return to;
};
var __toCommonJS = (mod) => __copyProps(__defProp({}, "__esModule", { value: true }), mod);

// src/index.ts
var index_exports = {};
__export(index_exports, {
  create_api_client: () => create_api_client,
  test_from_content_api_client: () => test_from_content_api_client
});
module.exports = __toCommonJS(index_exports);
function test_from_content_api_client() {
  console.log("Content API Client Loaded");
  return true;
}
function create_api_client(endpoint_base) {
  return "API client for " + endpoint_base;
}
// Annotate the CommonJS export names for ESM import in node:
0 && (module.exports = {
  create_api_client,
  test_from_content_api_client
});
