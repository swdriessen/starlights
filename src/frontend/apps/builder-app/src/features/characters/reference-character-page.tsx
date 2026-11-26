import { useCallback } from "react";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { Alert, AlertDescription, AlertTitle } from "@starlights/ui-framework/alert";
import { Badge } from "@starlights/ui-framework/badge";
import { Button } from "@starlights/ui-framework/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@starlights/ui-framework/card";
import { Field, FieldDescription, FieldGroup, FieldLabel } from "@starlights/ui-framework/field";
import { Input } from "@starlights/ui-framework/input";
import { NativeSelect, NativeSelectOption } from "@starlights/ui-framework/native-select";
import { characterBuildKey, getCharacterBuild, submitCharacterEdit, type CharacterBuild, type CharacterEdit } from "./character-build-api";
import { charactersKey } from "./characters-api";
import { CharacterError, CharacterLoading } from "./character-feedback";
import { CharacterDescription } from "./character-description";
import { useCharacterNotifications } from "./use-character-notifications";
import { useReliableEdits } from "./use-reliable-edits";

export function ReferenceCharacterPage({ id }: { id: string }) {
  const query = useQuery({
    queryKey: characterBuildKey(id), queryFn: ({ signal }) => getCharacterBuild(id, signal),
    retry: false, refetchInterval: 5_000, refetchOnWindowFocus: true,
  });
  return <>
    <Button variant="ghost" nativeButton={false} render={<Link to="/characters" />} className="self-start">Your characters</Button>
    {query.data ? <ReferenceEditor key={id} snapshot={query.data} queryFailed={query.isError} id={id} />
      : query.isPending ? <CharacterLoading />
      : <CharacterError title="This build could not be loaded" retry={() => void query.refetch()}>
        No replacement build has been created. Check your connection and try again.
      </CharacterError>}
  </>;
}

function ReferenceEditor({ id, snapshot, queryFailed }: { id: string; snapshot: CharacterBuild; queryFailed: boolean }) {
  const client = useQueryClient();
  const refresh = useCallback(() => client.fetchQuery({
    queryKey: characterBuildKey(id), queryFn: () => getCharacterBuild(id), staleTime: 0,
  }), [client, id]);
  const submit = useCallback((edit: { expectedRevision: number; operationId: string; action: CharacterEdit }) => submitCharacterEdit(id, edit), [id]);
  const accepted = useCallback(() => { void client.invalidateQueries({ queryKey: charactersKey }); }, [client]);
  const editor = useReliableEdits(snapshot, submit, refresh, accepted);
  const notifications = useCharacterNotifications(id, editor.retry, editor.reload);
  const { saved, active, drafts } = editor.state;
  const intent = [...(active ? [active] : []), ...drafts];
  const local = (key: string) => [...intent].reverse().find((item) => item.key === key)?.action;
  const nameEdit = local("name");
  const name = nameEdit?.kind === "name" ? nameEdit.name : saved.name;
  const abilityEdit = local("abilities");
  const inputs: Record<string, number> = abilityEdit?.kind === "abilities" ? abilityEdit.inputs
    : Object.fromEntries(saved.abilities.flatMap((ability) => ability.input == null ? [] : [[ability.id, ability.input]]));
  const equipmentEdit = local("equipment");
  const armorId = equipmentEdit?.kind === "equipment" ? equipmentEdit.armorId : saved.armorId;
  const weaponId = equipmentEdit?.kind === "equipment" ? equipmentEdit.weaponId : saved.weaponId;
  const unsaved = Boolean(active || drafts.length);
  const status = active?.phase === "sending" ? "Saving changes..."
    : active?.phase === "unconfirmed" ? "Save outcome unconfirmed"
    : active?.phase === "conflict" ? "Unsaved input / revision conflict"
    : active?.phase === "rejected" ? "Unsaved input / edit rejected"
    : drafts.length ? "Unsaved changes"
    : saved.status === "pending" ? "Changes saved / calculation pending"
    : saved.status === "failed" ? "Changes saved / calculation failed" : "All changes saved";
  const observeFailure = (action: () => Promise<unknown>) => { void action().catch(() => { /* The reload error is rendered below. */ }); };

  return <>
    <div className="flex flex-wrap items-start justify-between gap-4">
      <div className="flex min-w-0 flex-col gap-2">
        <p className="text-sm font-medium text-primary">Character workshop</p>
        <h1 className="break-words text-4xl font-semibold tracking-tight">{name || "The Nameless One"}</h1>
      </div>
      <div className="flex flex-wrap gap-2">
        <Badge variant="secondary">{unsaved ? "Saved build: " : ""}{saved.buildComplete ? "Complete" : "Incomplete"}</Badge>
        <Badge variant="outline">Revision {saved.revision}</Badge>
      </div>
    </div>
    <Alert role="status" aria-live="polite">
      <AlertTitle>{status}</AlertTitle>
      <AlertDescription>
        {unsaved ? "Your local input is retained below. The sheet describes saved facts, not unconfirmed changes."
          : saved.status === "failed" ? saved.failure
          : saved.status === "pending" ? "The server has accepted your changes. Returning later will show their calculation result or failure."
          : "Your choices are saved on the server. Incomplete decisions remain visible."}
      </AlertDescription>
    </Alert>
    {(queryFailed || editor.refreshError) && <CharacterError title="Latest saved state could not be checked" retry={() => observeFailure(editor.reload)}>
      {editor.refreshError ?? "The last loaded revision is shown. Your local input has not been replaced."}
    </CharacterError>}
    {notifications.connectionError && <Alert>
      <AlertTitle>Live update connection</AlertTitle>
      <AlertDescription>{notifications.connectionError}<Button variant="outline" onClick={notifications.reconnect}>Retry live updates</Button></AlertDescription>
    </Alert>}
    {active && active.phase !== "sending" && <Alert variant="destructive">
      <AlertTitle>{active.phase === "conflict" ? "Resolve the conflicting edit" : active.phase === "rejected" ? "Edit not accepted" : "Confirm the save outcome"}</AlertTitle>
      <AlertDescription>
        {active.message}
        <div className="flex flex-wrap gap-2">
          {active.phase === "unconfirmed" ? <Button variant="outline" onClick={() => observeFailure(editor.retry)}>Retry the same operation</Button>
            : <>
              <Button variant="outline" onClick={() => observeFailure(editor.reapply)}>Reload latest and reapply my input</Button>
              <Button variant="outline" onClick={editor.discard}>Discard my unsaved input</Button>
            </>}
        </div>
      </AlertDescription>
    </Alert>}
    <div className="grid items-start gap-6 xl:grid-cols-[minmax(0,1fr)_minmax(0,1fr)]">
      <div className="flex min-w-0 flex-col gap-6">
        <Card>
          <CardHeader><CardTitle>Your character</CardTitle><CardDescription>Changes save automatically after a short pause.</CardDescription></CardHeader>
          <CardContent>
            <FieldGroup>
              <Field data-invalid={name.length > 100}>
                <FieldLabel htmlFor="edit-character-name">Character name</FieldLabel>
                <Input id="edit-character-name" value={name} aria-invalid={name.length > 100} autoComplete="off"
                  onChange={(event) => editor.edit("name", { kind: "name", name: event.target.value })} />
                <FieldDescription>Up to 100 characters. Leave blank to use The Nameless One.</FieldDescription>
              </Field>
            </FieldGroup>
          </CardContent>
          <CardFooter><p className="text-xs text-muted-foreground">Ruleset and exact content version remain fixed.</p></CardFooter>
        </Card>
        <Card>
          <CardHeader><CardTitle>Build decisions</CardTitle><CardDescription>Options come from the Character's pinned content, not a prefilled reference character.</CardDescription></CardHeader>
          <CardContent>
            <FieldGroup>
              {saved.selections.map((selection) => {
                const edit = local(`choice:${selection.id}`);
                const answers = edit?.kind === "choice" ? edit.answers : selection.answers;
                return Array.from({ length: selection.count }, (_, index) => <Field key={`${selection.id}:${index}`}>
                  <FieldLabel htmlFor={`choice-${selection.id}-${index}`}>{selection.name}{selection.count > 1 ? ` (${index + 1} of ${selection.count})` : ""}</FieldLabel>
                  <NativeSelect className="w-full min-w-0" id={`choice-${selection.id}-${index}`} value={answers[index] ?? ""} onChange={(event) => {
                    const updated = Array.from({ length: selection.count }, (_, position) => position === index ? event.target.value : answers[position] ?? "").filter(Boolean);
                    editor.edit(`choice:${selection.id}`, { kind: "choice", selectionId: selection.id, answers: updated });
                  }}>
                    <NativeSelectOption value="">Not chosen</NativeSelectOption>
                    {selection.options.map((option) => <NativeSelectOption key={option.id} value={option.id}
                      disabled={answers.some((answer, position) => position !== index && answer === option.id)}>{option.name}</NativeSelectOption>)}
                  </NativeSelect>
                  {selection.options.find((option) => option.id === answers[index])?.description && <CharacterDescription>
                    {selection.options.find((option) => option.id === answers[index])?.description ?? ""}
                  </CharacterDescription>}
                </Field>);
              })}
            </FieldGroup>
          </CardContent>
          <CardFooter><p className="text-xs text-muted-foreground">Unanswered choices are build work, not invented benefits.</p></CardFooter>
        </Card>
        <Card>
          <CardHeader><CardTitle>Ability scores</CardTitle><CardDescription>Assign the standard array. Each value can be used once.</CardDescription></CardHeader>
          <CardContent>
            <FieldGroup>
              {saved.abilities.map((ability) => <Field key={ability.id}>
                <FieldLabel htmlFor={`ability-${ability.id}`}>{ability.name}</FieldLabel>
                <NativeSelect className="w-full min-w-0" id={`ability-${ability.id}`} value={inputs[ability.id] ?? ""} onChange={(event) => {
                  const updated = { ...inputs };
                  if (event.target.value) updated[ability.id] = Number(event.target.value);
                  else delete updated[ability.id];
                  editor.edit("abilities", { kind: "abilities", inputs: updated });
                }}>
                  <NativeSelectOption value="">Unassigned</NativeSelectOption>
                  {saved.abilityInputOptions.map((value) => <NativeSelectOption key={value} value={value}
                    disabled={Object.entries(inputs).some(([id, input]) => id !== ability.id && input === value)}>{value}</NativeSelectOption>)}
                </NativeSelect>
                <FieldDescription>Saved effective score: {ability.score ?? "unavailable"} / modifier: {ability.modifier ?? "unavailable"}</FieldDescription>
              </Field>)}
            </FieldGroup>
          </CardContent>
          <CardFooter><p className="text-xs text-muted-foreground">Recorded input stays distinct from content-derived increases.</p></CardFooter>
        </Card>
        <Card>
          <CardHeader><CardTitle>Starting equipment</CardTitle><CardDescription>Choosing a package does not acquire it. Claim each package separately.</CardDescription></CardHeader>
          <CardContent className="flex flex-col gap-5">
            {saved.equipmentClaims.map((claim) => <div key={claim.selectionId} className="flex flex-wrap items-center justify-between gap-2">
              <span className="text-sm">{claim.name}</span>
              <Button variant="outline" disabled={claim.claimed || Boolean(local(`claim:${claim.selectionId}`))}
                onClick={() => editor.edit(`claim:${claim.selectionId}`, { kind: "claim", selectionId: claim.selectionId })}>
                {claim.claimed ? "Claimed" : local(`claim:${claim.selectionId}`) ? "Claim pending" : "Claim package"}
              </Button>
            </div>)}
            <ul className="flex flex-col gap-2 text-sm">{saved.ownedItems.map((item) => <li key={item.id}>{item.quantity} × {item.name}</li>)}</ul>
            <FieldGroup>
              <Field>
                <FieldLabel htmlFor="equipped-armor">Wear armor</FieldLabel>
                <NativeSelect className="w-full min-w-0" id="equipped-armor" value={armorId ?? ""} onChange={(event) => editor.edit("equipment", {
                  kind: "equipment", armorId: event.target.value || null, weaponId: weaponId ?? null,
                })}>
                  <NativeSelectOption value="">No armor worn</NativeSelectOption>
                  {saved.ownedItems.filter((item) => item.canWear).map((item) => <NativeSelectOption key={item.id} value={item.id}>{item.name}</NativeSelectOption>)}
                </NativeSelect>
              </Field>
              <Field>
                <FieldLabel htmlFor="equipped-weapon">Wield weapon</FieldLabel>
                <NativeSelect className="w-full min-w-0" id="equipped-weapon" value={weaponId ?? ""} onChange={(event) => editor.edit("equipment", {
                  kind: "equipment", armorId: armorId ?? null, weaponId: event.target.value || null,
                })}>
                  <NativeSelectOption value="">No weapon wielded</NativeSelectOption>
                  {saved.ownedItems.filter((item) => item.canWield).map((item) => <NativeSelectOption key={item.id} value={item.id}>{item.name}</NativeSelectOption>)}
                </NativeSelect>
              </Field>
            </FieldGroup>
          </CardContent>
          <CardFooter><p className="text-xs text-muted-foreground">Only eligible owned items can be equipped. Repeated claims do not duplicate possessions or currency.</p></CardFooter>
        </Card>
      </div>
      <Card>
        <CardHeader>
          <CardTitle>Character sheet</CardTitle>
          <CardDescription>{unsaved ? `Saved revision ${saved.revision}; local edits are not reflected yet.` : `Calculation for saved revision ${saved.revision}.`}</CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-6">
          {!saved.sheetReady || !saved.sheet ? <Alert>
            <AlertTitle>{saved.status === "failed" ? "Calculation failed" : "Sheet not ready"}</AlertTitle>
            <AlertDescription>{saved.failure ?? "No unresolved or stale result is presented as a current statistic."}</AlertDescription>
          </Alert> : <>
            <dl className="grid gap-4 sm:grid-cols-2">
              {saved.sheet.statistics.map((statistic) => <div key={statistic.id} className="flex flex-col gap-1">
                <dt className="text-sm text-muted-foreground">{statistic.name}</dt>
                <dd className="font-semibold">{statistic.value ?? "Unavailable"}</dd>
                <dd className="break-all text-xs text-muted-foreground">{statistic.sources.join(" · ")}</dd>
              </div>)}
            </dl>
            <div className="flex flex-col gap-3">
              <h2 className="font-semibold">Attacks</h2>
              {saved.sheet.attacks.map((attack, index) => <p key={`${attack.name}:${index}`} className="text-sm">
                {attack.name}: {attack.toHit >= 0 ? "+" : ""}{attack.toHit} to hit / {attack.damage}{attack.mastery ? ` / ${attack.mastery}` : ""}
              </p>)}
            </div>
            <div className="flex flex-col gap-3">
              <h2 className="font-semibold">Features and reminders</h2>
              {saved.sheet.entries.map((entry) => <div key={entry.id} className="flex flex-col gap-1">
                <h3 className="text-sm font-medium">{entry.name}</h3>
                {entry.description && <CharacterDescription>{entry.description}</CharacterDescription>}
                {entry.usage && <p className="text-xs text-muted-foreground">{entry.usage}</p>}
              </div>)}
            </div>
            <p className="font-medium">{saved.sheet.gold} GP</p>
          </>}
        </CardContent>
        <CardFooter><p className="text-xs text-muted-foreground">Build completion, save status and sheet readiness are separate.</p></CardFooter>
      </Card>
    </div>
  </>;
}
