import { ComponentExample } from "@/components/component-example";
import { abilityScoresHooks, type AbilityScoresHooks } from "@starlights/content-api";
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from "./components/ui/select";

export const API_BASE = import.meta.env.VITE_CONTENT_API_ENDPOINT;

//test
export function App() {
  const hooks: AbilityScoresHooks = abilityScoresHooks;
  const { data } = hooks.useAbilityScores();

  const sortedAbilityScores = data?.items ? [...data.items].sort((a, b) => a.name.localeCompare(b.name, undefined, { sensitivity: "base" })) : undefined;

  const items = sortedAbilityScores?.map((x) => {
    return {
      label: x.name,
      value: x.id,
    };
  });

  if (data && data.items.length > 0 && items && items.length > 0) {
    return (
      <>
        <div className="p-4 border border-dashed rounded-2xl m-50">
          <Select
            items={items}
            onValueChange={(e) => {
              const selected = data.items.find((item) => item.id === e);
              console.log("selected ability score:", selected);
            }}
          >
            <SelectTrigger>
              <SelectValue />
            </SelectTrigger>

            <SelectContent>
              <SelectGroup>
                {sortedAbilityScores?.map((abilityScore) => (
                  <SelectItem key={abilityScore.id} value={abilityScore.id}>
                    {abilityScore.name}
                  </SelectItem>
                ))}
              </SelectGroup>
            </SelectContent>
          </Select>
        </div>
      </>
    );
  }

  return <ComponentExample />;
}

export default App;
