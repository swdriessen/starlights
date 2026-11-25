import { ApiError, type components } from "@starlights/api-client";
import { generatedApi } from "@/lib/api-client";

type FileResponse = components["schemas"]["FileResponse"];
export type PrivateFile = Omit<FileResponse, "sizeBytes"> & { sizeBytes: number };
export const filesKey = ["private-files"] as const;
export const maximumFileBytes = 5 * 1024 * 1024;

export async function listFiles(offset: number, signal: AbortSignal) {
  const { data, error, response } = await generatedApi.GET("/api/files", { params: { query: { offset } }, signal });
  checkResponse(response, error);
  if (!data) throw new Error("The file library response is incomplete.");
  return {
    files: data.files.map(normalizeFile),
    nextOffset: data.nextOffset == null ? null : nonNegativeInteger(data.nextOffset, "nextOffset"),
  };
}

export async function uploadFile(file: File) {
  if (file.size === 0 || file.size > maximumFileBytes) throw new Error("Choose a non-empty image no larger than 5 MiB.");
  const contentBase64 = await new Promise<string>((resolve, reject) => {
    const reader = new FileReader();
    reader.onerror = () => reject(new Error("Unable to read the selected file. Choose it again."));
    reader.onabort = () => reject(new Error("Reading the selected file was cancelled."));
    reader.onload = () => {
      if (typeof reader.result !== "string" || !reader.result.includes(",")) {
        reject(new Error("Unable to encode the selected file."));
        return;
      }
      resolve(reader.result.slice(reader.result.indexOf(",") + 1));
    };
    reader.readAsDataURL(file);
  });
  const { data, error, response } = await generatedApi.POST("/api/files", { body: { fileName: file.name, contentBase64 } });
  checkResponse(response, error);
  if (!data) throw new Error("The upload response is incomplete. Refresh the library before retrying.");
  return normalizeFile(data);
}

export async function deleteFile(id: string) {
  const { error, response } = await generatedApi.DELETE("/api/files/{fileId}", { params: { path: { fileId: id } } });
  checkResponse(response, error);
}

function checkResponse(response: Response, error: unknown) {
  if (response.ok) return;
  if (error && typeof error === "object" && "detail" in error && typeof error.detail === "string")
    throw new Error(error.detail);
  throw new ApiError(response.status, response.statusText, JSON.stringify(error ?? null));
}

function normalizeFile(file: FileResponse): PrivateFile {
  return { ...file, sizeBytes: nonNegativeInteger(file.sizeBytes, "sizeBytes") };
}

function nonNegativeInteger(value: number | string, field: string): number {
  const parsed = Number(value);
  if (!Number.isSafeInteger(parsed) || parsed < 0) throw new Error(`The file service returned an invalid ${field}.`);
  return parsed;
}
