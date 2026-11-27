import { zodResolver } from "@hookform/resolvers/zod";
import { ApiError } from "@starlights/api-client";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowLeftIcon, ArrowRightIcon, BookOpenIcon, ShieldCheckIcon } from "lucide-react";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router-dom";
import { z } from "zod";
import { Alert, AlertDescription, AlertTitle } from "@starlights/ui-framework/alert";
import { Badge } from "@starlights/ui-framework/badge";
import { Button } from "@starlights/ui-framework/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@starlights/ui-framework/card";
import { Field, FieldDescription, FieldError, FieldGroup, FieldLabel } from "@starlights/ui-framework/field";
import { Input } from "@starlights/ui-framework/input";
import { NativeSelect, NativeSelectOption } from "@starlights/ui-framework/native-select";
import { charactersKey, createCharacter, getRulesetOptions, type RulesetOption } from "./characters-api";
import { CharacterError, CharacterLoading } from "./character-feedback";

const schema = z.object({ name: z.string().max(100, "Use a name of 100 characters or fewer."), option: z.string() });

export function CreateCharacterPage() {
  const options = useQuery({ queryKey: ["character-ruleset-options"], queryFn: ({ signal }) => getRulesetOptions(signal) });
  return <>
    <Button variant="ghost" nativeButton={false} render={<Link to="/characters" />} className="self-start">
      <ArrowLeftIcon data-icon="inline-start" /> Your characters
    </Button>
    <div className="flex flex-col gap-2">
      <p className="text-sm font-medium text-primary">Start something new</p>
      <h1 className="text-3xl font-semibold tracking-tight">Create a character</h1>
      <p className="text-muted-foreground">A name, a Ruleset, and a place to begin.</p>
    </div>
    {options.isPending ? <CharacterLoading /> : options.isError
      ? <CharacterError title="Creation content is unavailable" retry={() => void options.refetch()}>
          We could not load supported Rulesets and content. Nothing has been created.
        </CharacterError>
      : <CreationForm options={options.data} />}
  </>;
}

function CreationForm({ options }: { options: RulesetOption[] }) {
  const navigate = useNavigate();
  const client = useQueryClient();
  const form = useForm<z.infer<typeof schema>>({ resolver: zodResolver(schema), defaultValues: { name: "", option: "0" } });
  const selected = options[Number(form.watch("option"))];
  const mutation = useMutation({
    mutationFn: createCharacter,
    retry: false,
    onSettled: () => {
      void client.invalidateQueries({ queryKey: charactersKey });
    },
    onSuccess: (id) => navigate(`/characters/${id}`),
  });
  const invalidName = Boolean(form.formState.errors.name);
  const rejected = mutation.error instanceof ApiError && mutation.error.status === 400;

  return <div className="grid items-start gap-6 lg:grid-cols-[minmax(0,1.4fr)_minmax(0,1fr)]">
    <Card>
      <CardHeader>
        <CardTitle>The beginning of your story</CardTitle>
        <CardDescription>Your Character is saved when you create it. Any starting ability assignments remain editable.</CardDescription>
      </CardHeader>
      <CardContent>
        <form id="create-character" onSubmit={form.handleSubmit(({ name }) => {
          if (!mutation.isPending && selected) mutation.mutate({ name, option: selected });
        })} className="flex flex-col gap-6">
          <FieldGroup>
            <Field data-invalid={invalidName} data-disabled={mutation.isPending}>
              <FieldLabel htmlFor="character-name">Character name</FieldLabel>
              <Input id="character-name" placeholder="The Nameless One" autoComplete="off" {...form.register("name")}
                disabled={mutation.isPending} aria-invalid={invalidName} aria-describedby="character-name-help character-name-error" />
              <FieldDescription id="character-name-help">Up to 100 characters. Leave blank to use The Nameless One. Names do not have to be unique.</FieldDescription>
              <FieldError id="character-name-error" errors={[form.formState.errors.name]} />
            </Field>
            <Field data-disabled={mutation.isPending}>
              <FieldLabel htmlFor="character-ruleset">Ruleset and content</FieldLabel>
              <NativeSelect id="character-ruleset" {...form.register("option")} disabled={mutation.isPending} aria-describedby="ruleset-help">
                {options.map((option, index) => <NativeSelectOption key={`${option.rulesetId}:${option.content.characterElementId}:${option.content.packVersion}`} value={String(index)}>
                  {option.name} / {option.packName} v{option.content.packVersion}
                </NativeSelectOption>)}
              </NativeSelect>
              <FieldDescription id="ruleset-help">The Ruleset stays fixed. This exact content version is pinned when you create the Character.</FieldDescription>
            </Field>
          </FieldGroup>
          {mutation.isError && <CharacterError title={rejected ? "Character not created" : "Creation could not be confirmed"}>
            {rejected ? "Check the name and selected content, then try again. Your input is still here."
              : <>The request may have reached the server. <Link to="/characters" className="underline underline-offset-4">Check your collection</Link> before trying again to avoid creating a duplicate. Your entered name is still here.</>}
          </CharacterError>}
          <p role="status" aria-live="polite" className="text-sm text-muted-foreground">
            {mutation.isPending ? "Creating and saving your Character..." : "Nothing is created until you choose Create character."}
          </p>
        </form>
      </CardContent>
      <CardFooter className="flex flex-wrap justify-between gap-3">
        <Button variant="outline" nativeButton={false} render={<Link to="/characters" />} disabled={mutation.isPending}>Cancel</Button>
        <Button type="submit" form="create-character" disabled={mutation.isPending || !selected}>
          {mutation.isPending ? "Saving character..." : "Create character"} <ArrowRightIcon data-icon="inline-end" />
        </Button>
      </CardFooter>
    </Card>
    <div className="flex flex-col gap-5">
      <Card>
        <CardHeader>
          <CardDescription>Selected foundation</CardDescription>
          <CardTitle>{selected?.packName}</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-col gap-4">
          <div className="flex flex-wrap gap-2"><Badge variant="outline">{selected?.rulesetId}</Badge><Badge variant="secondary">Pinned content</Badge></div>
          <p className="text-sm text-muted-foreground">Begin with an incomplete character, then make the choices supported by this content. No reference build is filled in for you.</p>
        </CardContent>
        <CardFooter><p className="text-xs text-muted-foreground">Content pack version {selected?.content.packVersion}</p></CardFooter>
      </Card>
      <Alert>
        <ShieldCheckIcon aria-hidden="true" />
        <AlertTitle>Saved privately, on the server</AlertTitle>
        <AlertDescription>Only your signed-in account can list and reopen this Character. It is not stored only in this browser.</AlertDescription>
      </Alert>
      <p className="flex items-start gap-2 text-sm text-muted-foreground"><BookOpenIcon className="mt-0.5 size-4 shrink-0" aria-hidden="true" />An incomplete build is a saved beginning, not a finished character sheet.</p>
    </div>
  </div>;
}
