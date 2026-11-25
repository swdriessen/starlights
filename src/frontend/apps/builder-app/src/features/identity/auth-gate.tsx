import { QueryClient, QueryClientProvider, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useEffect, useState } from "react";
import { Link, Outlet, useNavigate } from "react-router-dom";
import { Button } from "@starlights/ui-framework/button";
import { FieldError } from "@starlights/ui-framework/field";
import { getSession, sessionKey, signOut, type Session } from "./session-api";
import { SignInPage } from "./sign-in-page";

export function AuthGate() {
  const client = useQueryClient();
  const navigate = useNavigate();
  const session = useQuery({ queryKey: sessionKey, queryFn: ({ signal }) => getSession(signal), retry: false, refetchOnWindowFocus: true });

  useEffect(() => {
    const ended = () => {
      void client.cancelQueries({ queryKey: sessionKey });
      client.setQueryData(sessionKey, null);
    };
    window.addEventListener("starlights:session-ended", ended);
    return () => window.removeEventListener("starlights:session-ended", ended);
  }, [client]);

  if (session.isPending) return <main className="container mx-auto p-8" role="status">Checking your session...</main>;
  if (session.isError) return <main className="container mx-auto flex flex-col gap-4 p-8">
    <p role="alert">Unable to check your session. Check your connection and try again.</p>
    <Button onClick={() => void session.refetch()}>Retry</Button>
  </main>;
  if (!session.data) return <SignInPage onSignedIn={(account) => {
    void client.cancelQueries({ queryKey: sessionKey });
    client.setQueryData(sessionKey, account);
    navigate("/", { replace: true });
  }} />;

  return <SignedIn key={session.data.userId} session={session.data} onSignedOut={() => {
    void client.cancelQueries({ queryKey: sessionKey });
    client.setQueryData(sessionKey, null);
    navigate("/", { replace: true });
  }} />;
}

function SignedIn({ session, onSignedOut }: { session: Session; onSignedOut: () => void }) {
  const [privateQueries] = useState(() => new QueryClient({
    defaultOptions: { queries: { retry: 1, staleTime: 5_000, refetchOnWindowFocus: false } },
  }));
  const mutation = useMutation({ mutationFn: signOut, onSuccess: onSignedOut });
  useEffect(() => () => {
    void privateQueries.cancelQueries();
    privateQueries.clear();
  }, [privateQueries]);

  return <QueryClientProvider client={privateQueries}>
    <div className="flex flex-wrap items-center justify-end gap-4 border-b px-4 py-2">
      <Link to="/files" className="text-sm underline underline-offset-4">Private files</Link>
      <span className="text-sm">Signed in as {session.username}</span>
      <Button variant="outline" size="sm" onClick={() => mutation.mutate()} disabled={mutation.isPending}>
        {mutation.isPending ? "Signing out..." : "Sign out"}
      </Button>
      {mutation.isError && <FieldError role="alert">Unable to sign out. Check your connection and try again.</FieldError>}
    </div>
    <Outlet />
  </QueryClientProvider>;
}
