import { ApiError } from "@starlights/api-client";
import { useQuery } from "@tanstack/react-query";
import { ArrowLeftIcon, CheckCircle2Icon, CircleDashedIcon } from "lucide-react";
import { Link, useParams } from "react-router-dom";
import { Alert, AlertDescription, AlertTitle } from "@starlights/ui-framework/alert";
import { Badge } from "@starlights/ui-framework/badge";
import { Button } from "@starlights/ui-framework/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@starlights/ui-framework/card";
import { characterKey, getCharacter } from "./characters-api";
import { CharacterError, CharacterLoading } from "./character-feedback";
import { ReferenceCharacterPage } from "./reference-character-page";

export function CharacterOverviewPage() {
  const { id = "" } = useParams();
  const saved = useQuery({ queryKey: characterKey(id), queryFn: ({ signal }) => getCharacter(id, signal), retry: false });

  if (saved.data?.buildSupported) return <ReferenceCharacterPage id={id} />;

  return <>
    <Button variant="ghost" nativeButton={false} render={<Link to="/characters" />} className="self-start">
      <ArrowLeftIcon data-icon="inline-start" /> Your characters
    </Button>
    {saved.isPending ? <CharacterLoading /> : saved.isError
      ? <CharacterError title={saved.error instanceof ApiError && saved.error.status === 404 ? "Character not found" : "This Character could not be reopened"}
          retry={() => void saved.refetch()}>
          The Character may be unavailable or belong to another account. No replacement or reset has been made.
        </CharacterError>
      : <>
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div className="flex min-w-0 flex-col gap-3">
            <p className="text-sm font-medium text-primary">Character overview</p>
            <h1 className="break-words text-4xl font-semibold tracking-tight">{saved.data.name}</h1>
            <p className="text-muted-foreground">{saved.data.rulesetId ?? "Retained legacy Character"}</p>
          </div>
          <Badge variant="secondary">{saved.data.rulesetId ? "Incomplete build" : "Legacy build"}</Badge>
        </div>
        {saved.data.rulesetId ? <>
          <Alert role="status">
            <CheckCircle2Icon aria-hidden="true" />
            <AlertTitle>Saved and ready to return to</AlertTitle>
            <AlertDescription>This overview was loaded from the server. Your name, Ruleset and starting state are saved, even though the build is incomplete.</AlertDescription>
          </Alert>
          <div className="grid items-start gap-6 md:grid-cols-2">
            <Card>
              <CardHeader>
                <CardTitle>A foundation for your character</CardTitle>
                <CardDescription>The exact content version selected at creation.</CardDescription>
              </CardHeader>
              <CardContent>
                <dl className="grid gap-4 text-sm">
                  <div className="flex flex-col gap-1"><dt className="text-muted-foreground">Ruleset</dt><dd>{saved.data.rulesetId}</dd></div>
                  <div className="flex flex-col gap-1"><dt className="text-muted-foreground">Content pack</dt><dd>{saved.data.content?.packId}</dd></div>
                  <div className="flex flex-col gap-1"><dt className="text-muted-foreground">Pinned version</dt><dd>{saved.data.content?.packVersion}</dd></div>
                  <div className="flex flex-col gap-1"><dt className="text-muted-foreground">Character Element</dt><dd className="break-all font-mono text-xs">{saved.data.content?.characterElementId}</dd></div>
                </dl>
              </CardContent>
              <CardFooter><p className="text-xs text-muted-foreground">Newer content will not silently replace this version.</p></CardFooter>
            </Card>
            <Card>
              <CardHeader>
                <CardTitle>Your next chapter</CardTitle>
                <CardDescription>Building features are coming in later slices.</CardDescription>
              </CardHeader>
              <CardContent className="flex flex-col gap-4">
                <p className="flex items-start gap-2 text-sm text-muted-foreground"><CircleDashedIcon className="size-4 shrink-0" aria-hidden="true" />Species, background, class and ability choices have not been recorded.</p>
                <p className="text-sm text-muted-foreground">Equipment and sheet calculations are not available yet. No missing values are presented as final statistics.</p>
              </CardContent>
              <CardFooter><Badge variant="outline">Build incomplete / sheet not calculated</Badge></CardFooter>
            </Card>
          </div>
        </> : <Alert>
          <CircleDashedIcon aria-hidden="true" />
          <AlertTitle>Original legacy state retained</AlertTitle>
          <AlertDescription>
            This Character has not been assigned a guessed Ruleset or content version.
            <Button variant="outline" nativeButton={false} render={<Link to={`/characters/${id}/builder`} />}>Open legacy builder</Button>
          </AlertDescription>
        </Alert>}
        <p className="break-all text-xs text-muted-foreground">Character ID: {saved.data.characterId}</p>
      </>}
  </>;
}
