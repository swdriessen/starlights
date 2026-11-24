import { zodResolver } from "@hookform/resolvers/zod";
import { ApiError } from "@starlights/api-client";
import { useMutation } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { signIn, type Session } from "./session-api";

const schema = z.object({
  username: z.string().min(1, "Enter your username.").max(256),
  password: z.string().min(1, "Enter your password.").max(1024),
});

export function SignInPage({ onSignedIn }: { onSignedIn: (session: Session) => void }) {
  const form = useForm<z.infer<typeof schema>>({ resolver: zodResolver(schema), defaultValues: { username: "", password: "" } });
  const mutation = useMutation({ mutationFn: signIn, onSuccess: onSignedIn });

  return (
    <main className="container mx-auto flex min-h-screen items-center justify-center p-4">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle>Sign in to Starlights</CardTitle>
          <CardDescription>Use the account provided by your operator.</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={form.handleSubmit((values) => mutation.mutate(values))}>
            <FieldGroup>
              <Field data-invalid={Boolean(form.formState.errors.username)} data-disabled={mutation.isPending}>
                <FieldLabel htmlFor="username">Username</FieldLabel>
                <Input id="username" autoComplete="username" {...form.register("username")} disabled={mutation.isPending}
                  aria-invalid={Boolean(form.formState.errors.username)} aria-describedby="username-error" />
                <FieldError id="username-error" errors={[form.formState.errors.username]} />
              </Field>
              <Field data-invalid={Boolean(form.formState.errors.password)} data-disabled={mutation.isPending}>
                <FieldLabel htmlFor="password">Password</FieldLabel>
                <Input id="password" type="password" autoComplete="current-password" {...form.register("password")} disabled={mutation.isPending}
                  aria-invalid={Boolean(form.formState.errors.password)} aria-describedby="password-error" />
                <FieldError id="password-error" errors={[form.formState.errors.password]} />
              </Field>
              {mutation.isError && <FieldError role="alert">
                {mutation.error instanceof ApiError && mutation.error.status === 401
                  ? "Sign-in failed. Check your credentials or contact the operator."
                  : "Unable to sign in. Check your connection and try again."}
              </FieldError>}
              <Button type="submit" disabled={mutation.isPending}>{mutation.isPending ? "Signing in..." : "Sign in"}</Button>
            </FieldGroup>
          </form>
        </CardContent>
        <CardFooter><p className="text-sm text-muted-foreground">For a new account or password reset, contact your operator.</p></CardFooter>
      </Card>
    </main>
  );
}
