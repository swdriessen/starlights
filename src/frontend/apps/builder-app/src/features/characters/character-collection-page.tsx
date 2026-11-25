import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { ArrowRightIcon, BookOpenIcon, PlusIcon, RefreshCwIcon } from "lucide-react";
import { Badge } from "@starlights/ui-framework/badge";
import { Button } from "@starlights/ui-framework/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@starlights/ui-framework/card";
import { Empty, EmptyContent, EmptyDescription, EmptyHeader, EmptyMedia, EmptyTitle } from "@starlights/ui-framework/empty";
import { charactersKey, listCharacters } from "./characters-api";
import { CharacterError, CharacterLoading } from "./character-feedback";

export function CharacterCollectionPage() {
  const [offset, setOffset] = useState(0);
  const collection = useQuery({ queryKey: [...charactersKey, { offset }], queryFn: ({ signal }) => listCharacters(offset, signal) });

  return <>
    <div className="flex flex-wrap items-end justify-between gap-6">
      <div className="flex flex-col gap-2">
        <p className="text-sm font-medium text-primary">Your collection</p>
        <h1 className="text-3xl font-semibold tracking-tight sm:text-4xl">Your characters</h1>
        <p className="max-w-xl text-muted-foreground">Every story starts with a character. Keep yours here, ready to return to.</p>
      </div>
      <Button nativeButton={false} render={<Link to="/characters/create" />} size="lg">
        <PlusIcon data-icon="inline-start" /> New character
      </Button>
    </div>
    <div className="flex items-center justify-between gap-4 text-sm text-muted-foreground">
      <p>Private to your account</p>
      <Button variant="ghost" size="sm" onClick={() => void collection.refetch()} disabled={collection.isFetching}>
        <RefreshCwIcon data-icon="inline-start" /> Refresh collection
      </Button>
    </div>
    {collection.isPending ? <CharacterLoading /> : collection.isError
      ? <CharacterError title="Your collection could not be loaded" retry={() => void collection.refetch()}>
          Your Characters have not been replaced by an empty collection. Check your connection and try again.
        </CharacterError>
      : collection.data.characters.length === 0
        ? <Empty className="min-h-80 border">
            <EmptyHeader>
              <EmptyMedia variant="icon"><BookOpenIcon aria-hidden="true" /></EmptyMedia>
              <EmptyTitle>{offset === 0 ? "A new story awaits" : "No more characters on this page"}</EmptyTitle>
              <EmptyDescription>Create a Character with a name and a Ruleset. It will be saved here, even before its build is complete.</EmptyDescription>
            </EmptyHeader>
            <EmptyContent>
              <Button nativeButton={false} render={<Link to="/characters/create" />}>Create a character</Button>
              {offset > 0 && <Button variant="outline" onClick={() => setOffset(0)}>Back to first page</Button>}
            </EmptyContent>
          </Empty>
        : <div className="grid gap-5 md:grid-cols-2 lg:grid-cols-3">
            {collection.data.characters.map((character) => <Card key={character.characterId}>
              <CardHeader>
                <CardDescription>{character.rulesetId ?? "Legacy Character"}</CardDescription>
                <CardTitle className="min-w-0">
                  <Link to={`/characters/${character.characterId}`} className="break-words hover:underline underline-offset-4">{character.name}</Link>
                </CardTitle>
              </CardHeader>
              <CardContent className="flex flex-col items-start gap-4">
                <Badge variant="secondary">{character.rulesetId ? "Incomplete build" : "Legacy build"}</Badge>
                <p className="text-sm text-muted-foreground">{character.rulesetId
                  ? "Your starting point is saved. Building choices and sheet calculations are not available yet."
                  : "Saved using the previous builder. Its original content has not been silently converted."}</p>
              </CardContent>
              <CardFooter className="mt-auto">
                <Button variant="outline" nativeButton={false} render={<Link to={`/characters/${character.characterId}`} />}>
                  Open character <ArrowRightIcon data-icon="inline-end" />
                </Button>
              </CardFooter>
            </Card>)}
          </div>}
    {collection.data && (offset > 0 || collection.data.nextOffset !== null) && <nav aria-label="Character collection pages" className="flex justify-between gap-4">
      <Button variant="outline" disabled={offset === 0 || collection.isFetching} onClick={() => setOffset(Math.max(0, offset - 50))}>Previous</Button>
      <Button variant="outline" disabled={collection.data.nextOffset === null || collection.isFetching}
        onClick={() => { if (collection.data.nextOffset !== null) setOffset(collection.data.nextOffset); }}>Next</Button>
    </nav>}
  </>;
}
