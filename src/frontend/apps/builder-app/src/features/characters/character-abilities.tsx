import { useState } from "react";
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@starlights/ui-framework/alert-dialog";
import { Button } from "@starlights/ui-framework/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@starlights/ui-framework/card";
import { Field, FieldDescription, FieldGroup, FieldLabel } from "@starlights/ui-framework/field";
import { Input } from "@starlights/ui-framework/input";
import { NativeSelect, NativeSelectOption } from "@starlights/ui-framework/native-select";
import { ToggleGroup, ToggleGroupItem } from "@starlights/ui-framework/toggle-group";
import type { AbilityMethod, CharacterBuild, CharacterEdit } from "./character-build-api";

const methods: { id: AbilityMethod; label: string }[] = [
  { id: "standard-array", label: "Standard array" },
  { id: "point-buy", label: "Point buy" },
  { id: "application-rolls", label: "Application rolls" },
  { id: "physical-dice", label: "Physical dice" },
];

export function CharacterAbilities({ saved, inputs, edit, unsaved, generationPending }: {
  saved: CharacterBuild;
  inputs: Record<string, number>;
  edit: (key: string, action: CharacterEdit) => void;
  unsaved: boolean;
  generationPending: boolean;
}) {
  const [replacement, setReplacement] = useState<{ method: AbilityMethod; reroll: boolean } | null>(null);
  const generation = saved.abilityGeneration;
  const allocated = generation.method === "standard-array" || generation.method === "application-rolls";
  const changeInput = (id: string, value: string) => {
    const updated = { ...inputs };
    if (value === "") delete updated[id];
    else updated[id] = Number(value);
    edit("abilities", { kind: "abilities", inputs: updated });
  };

  return <Card>
    <CardHeader>
      <CardTitle>Ability scores</CardTitle>
      <CardDescription>Choose your table's method. Inputs, increases and effective scores remain separate.</CardDescription>
    </CardHeader>
    <CardContent className="flex flex-col gap-5">
      <FieldGroup>
        <Field data-disabled={unsaved}>
          <FieldLabel id="ability-method-label">Generation method</FieldLabel>
          <ToggleGroup aria-labelledby="ability-method-label" variant="outline" className="flex-wrap" disabled={unsaved}
            value={[generation.method]} onValueChange={(values) => {
              const method = methods.find((candidate) => candidate.id === values[0])?.id;
              if (method && method !== generation.method) setReplacement({ method, reroll: false });
            }}>
            {methods.map((method) => <ToggleGroupItem key={method.id} value={method.id}>{method.label}</ToggleGroupItem>)}
          </ToggleGroup>
          <FieldDescription>{generationPending ? "Generation change is awaiting save confirmation. Assignments unlock after it is saved."
            : unsaved ? "Finish or resolve pending edits before changing methods."
            : "Changing methods clears assignments after confirmation. Previously generated dice are retained."}</FieldDescription>
        </Field>
      </FieldGroup>
      {generation.method === "point-buy" && <p role="status" className="text-sm">
        Saved point buy: {generation.pointsSpent ?? "unavailable"} / 27 spent; {generation.pointsRemaining ?? "unavailable"} remaining.
        {" "}Scores 8-15 cost 0, 1, 2, 3, 4, 5, 7, 9. Unspent points are allowed.
      </p>}
      {generation.method === "application-rolls" && <>
        <p className="text-sm text-muted-foreground">Six saved 4d6 results, dropping one lowest die from each. Reopening and recalculation never reroll them.</p>
        <ol aria-label="Saved application rolls" className="flex flex-col gap-2 text-sm">
          {generation.rolls.map((roll, index) => <li key={index}>
            Roll {index + 1}: {roll.dice.join(" + ")}; drop {Math.min(...roll.dice)} = <strong>{roll.total}</strong>
          </li>)}
        </ol>
        <Button variant="outline" className="self-start" disabled={unsaved}
          onClick={() => setReplacement({ method: "application-rolls", reroll: true })}>Reroll scores</Button>
      </>}
      {generation.method === "physical-dice" && <p className="text-sm text-muted-foreground">
        Enter player-provided totals from 3 through 18. The physical rolling procedure is not verified by the application.
      </p>}
      <FieldGroup>
        {saved.abilities.map((ability) => {
          const input = inputs[ability.id];
          const invalid = input != null && (!Number.isInteger(input) || !saved.abilityInputOptions.includes(input));
          return <Field key={ability.id} data-invalid={invalid} data-disabled={generationPending}>
            <FieldLabel htmlFor={`ability-${ability.id}`}>{ability.name}</FieldLabel>
            {generation.method === "physical-dice" ? <Input type="number" min={3} max={18} step={1}
              id={`ability-${ability.id}`} value={input ?? ""} disabled={generationPending} aria-invalid={invalid}
              onChange={(event) => changeInput(ability.id, event.target.value)} />
              : <NativeSelect className="w-full min-w-0" id={`ability-${ability.id}`} value={input ?? ""}
                disabled={generationPending} aria-invalid={invalid} onChange={(event) => changeInput(ability.id, event.target.value)}>
                <NativeSelectOption value="">Unassigned</NativeSelectOption>
                {[...new Set(saved.abilityInputOptions)].map((value) => <NativeSelectOption key={value} value={value}
                  disabled={allocated && Object.entries(inputs).filter(([id, input]) => id !== ability.id && input === value).length
                    >= saved.abilityInputOptions.filter((available) => available === value).length}>{value}</NativeSelectOption>)}
              </NativeSelect>}
            <FieldDescription>
              {invalid && "Enter a whole-number total from 3 through 18. "}
              Saved input: {ability.input ?? "missing"}; before limit: {ability.ordinaryScore ?? "unavailable"}; maximum: {ability.maximum};
              {" "}effective: {ability.score ?? "unavailable"}; modifier: {ability.modifier ?? "unavailable"}.
              {ability.isFallback && " Fallback-derived, not an answered assignment."}
              {ability.issue && ` ${ability.issue}`}
            </FieldDescription>
            <details className="text-xs text-muted-foreground">
              <summary>Calculation sources for {ability.name}</summary>
              <ul className="flex flex-col gap-1">{ability.sources.map((source) => <li key={source} className="break-all">{source}</li>)}</ul>
            </details>
          </Field>;
        })}
      </FieldGroup>
      <AlertDialog open={replacement !== null} onOpenChange={(open) => { if (!open) setReplacement(null); }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{replacement?.reroll ? "Replace the saved rolls?" : "Change ability generation?"}</AlertDialogTitle>
            <AlertDialogDescription>
              {replacement?.reroll ? "Generate six new results and clear the current assignments. The old results will be replaced."
                : "Clear current assignments and use the selected method. Existing application dice are retained; choosing application rolls generates them only if none exist."}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction disabled={unsaved} onClick={() => {
              if (replacement) edit("ability-generation", { kind: "ability-generation", ...replacement, confirmReplacement: true });
            }}>Confirm replacement</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </CardContent>
    <CardFooter><p className="text-xs text-muted-foreground">All six valid assignments are required. Missing inputs use an explicitly labelled fallback, not a completed decision.</p></CardFooter>
  </Card>;
}
