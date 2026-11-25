import { useRef, useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription,
  AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger,
} from "@starlights/ui-framework/alert-dialog";
import { Button, buttonVariants } from "@starlights/ui-framework/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@starlights/ui-framework/card";
import { Empty, EmptyDescription, EmptyHeader, EmptyTitle } from "@starlights/ui-framework/empty";
import { Field, FieldDescription, FieldError, FieldGroup, FieldLabel } from "@starlights/ui-framework/field";
import { Input } from "@starlights/ui-framework/input";
import { deleteFile, filesKey, listFiles, uploadFile, type PrivateFile } from "./files-api";

export function PrivateFilesPage() {
  const client = useQueryClient();
  const input = useRef<HTMLInputElement>(null);
  const [selected, setSelected] = useState<File | null>(null);
  const [pages, setPages] = useState([0]);
  const [notice, setNotice] = useState("");
  const offset = pages[pages.length - 1];
  const library = useQuery({ queryKey: [...filesKey, offset], queryFn: ({ signal }) => listFiles(offset, signal) });
  const upload = useMutation({
    mutationFn: uploadFile,
    onSuccess: async () => {
      setSelected(null);
      if (input.current) input.current.value = "";
      setPages([0]);
      setNotice("Image uploaded.");
      await client.invalidateQueries({ queryKey: filesKey });
    },
  });
  const deletion = useMutation({
    mutationFn: deleteFile,
    onSuccess: () => setNotice("File deleted."),
    onSettled: () => client.invalidateQueries({ queryKey: filesKey }),
  });
  const busy = upload.isPending || deletion.isPending;

  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (selected) {
      setNotice("");
      upload.mutate(selected);
    }
  }

  return <section className="mx-auto flex max-w-6xl flex-col gap-6 pb-12">
    <header className="flex flex-col gap-2">
      <h1 className="text-3xl font-semibold">Private files</h1>
      <p className="text-muted-foreground">Your private image library. Only you can access these originals.</p>
    </header>
    <Card>
      <CardHeader>
        <CardTitle>Upload an image</CardTitle>
        <CardDescription>PNG, JPEG or WebP, up to 5 MiB and 4096 pixels per side. No animated images or SVG.</CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={submit} className="flex flex-col gap-4">
          <FieldGroup>
            <Field data-invalid={upload.isError}>
              <FieldLabel htmlFor="private-image">Image file</FieldLabel>
              <Input ref={input} id="private-image" type="file" accept="image/png,image/jpeg,image/webp" disabled={busy}
                aria-describedby="private-image-help" aria-invalid={upload.isError}
                onChange={(event) => {
                  setSelected(event.target.files?.[0] ?? null);
                  upload.reset();
                  setNotice("");
                }} />
              <FieldDescription id="private-image-help">Original bytes and embedded metadata are preserved. Uploading again creates a separate file.</FieldDescription>
              {upload.isError && <FieldError role="alert">{upload.error.message}</FieldError>}
            </Field>
          </FieldGroup>
          <Button type="submit" className="self-start" disabled={!selected || busy}>{upload.isPending ? "Uploading..." : "Upload image"}</Button>
        </form>
      </CardContent>
    </Card>
    <div role="status" aria-live="polite">{notice}</div>
    {deletion.isError && <FieldError role="alert">Unable to delete the file. Refresh the library and retry deletion. {deletion.error.message}</FieldError>}
    <div className="flex flex-wrap items-center justify-between gap-4">
      <h2 className="text-xl font-semibold">Your images</h2>
      <Button variant="outline" onClick={() => void library.refetch()} disabled={busy || library.isFetching}>
        {library.isFetching ? "Refreshing..." : "Refresh"}
      </Button>
    </div>
    {library.isPending && <p role="status">Loading your files...</p>}
    {library.isError && <FieldError role="alert">Unable to load your files. Check your connection and use Refresh to try again.</FieldError>}
    {library.isSuccess && library.data.files.length === 0 && <Empty>
      <EmptyHeader>
        <EmptyTitle>{offset === 0 ? "No files yet" : "No files on this page"}</EmptyTitle>
        <EmptyDescription>{offset === 0 ? "Upload your first image to get started. Character assignment is not available yet." : "Use Previous to return to your images."}</EmptyDescription>
      </EmptyHeader>
    </Empty>}
    {library.isSuccess && <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
      {library.data.files.map((file) => <PrivateFileCard key={file.id} file={file} busy={busy} onDelete={() => {
        setNotice("");
        deletion.mutate(file.id);
      }} />)}
    </div>}
    <nav aria-label="File pages" className="flex items-center justify-end gap-3">
      <Button variant="outline" disabled={pages.length === 1 || busy || library.isFetching} onClick={() => setPages((previous) => previous.slice(0, -1))}>Previous</Button>
      <span className="text-sm">Page {pages.length}</span>
      <Button variant="outline" disabled={library.data?.nextOffset == null || busy || library.isFetching} onClick={() => {
        const next = library.data?.nextOffset;
        if (next != null) setPages((previous) => [...previous, next]);
      }}>Next</Button>
    </nav>
  </section>;
}

function PrivateFileCard({ file, busy, onDelete }: { file: PrivateFile; busy: boolean; onDelete: () => void }) {
  const [previewFailed, setPreviewFailed] = useState(false);
  return <Card className="min-w-0">
    <CardHeader>
      <CardTitle className="break-all">{file.fileName}</CardTitle>
      <CardDescription>{(file.sizeBytes / 1024).toLocaleString(undefined, { maximumFractionDigits: 1 })} KiB - {file.contentType}</CardDescription>
    </CardHeader>
    <CardContent>
      {file.isDeleting ? <p role="status">Deletion is pending. Retry deletion to finish removing this file.</p>
        : previewFailed ? <p role="alert">Preview unavailable. Use View original to retry.</p>
          : <img src={file.contentUrl} alt={file.fileName} className="aspect-square w-full object-contain" loading="lazy" onError={() => setPreviewFailed(true)} />}
    </CardContent>
    <CardFooter className="mt-auto flex-wrap gap-3">
      {!file.isDeleting && <a className={buttonVariants({ variant: "outline" })} href={file.contentUrl} target="_blank" rel="noopener noreferrer">View original</a>}
      <AlertDialog>
        <AlertDialogTrigger render={<Button variant="destructive" disabled={busy} />}>{file.isDeleting ? "Retry deletion" : "Delete"}</AlertDialogTrigger>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete this private file?</AlertDialogTitle>
            <AlertDialogDescription>This permanently removes the original image. This cannot be undone.</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction onClick={onDelete}>Delete permanently</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </CardFooter>
  </Card>;
}
