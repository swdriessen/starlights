import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import {
  useAbilityScores,
  useCharacterCards,
  useSavingThrows,
  useSkills,
  useUpdateAdditionalAbilityScore,
  useUpdateBaseAbilityScore,
} from "@/lib/api/characters/queries";
import { ChevronDown, ChevronUp } from "lucide-react";
import { useParams } from "react-router-dom";

function AbilitiesComponent({ id: characterId }: { id: string }) {
  const { data: abilityScores } = useAbilityScores(characterId);
  const updateBaseAbilityScore = useUpdateBaseAbilityScore(characterId);
  const updateAdditionalAbilityScore = useUpdateAdditionalAbilityScore(characterId);

  return (
    <>
      <div className="flex flex-row items-center justify-between mb-2 border border-dashed rounded p-4">
        {abilityScores && (
          <div className="overflow-x-auto min-w-full">
            <h2 className="text-xs font-semibold uppercase text-center bg-accent p-2 rounded leading-6 tracking-widest">Ability Scores</h2>
            <Separator className="my-4" />

            <table className="min-w-full text-center">
              <thead className="text-xs  uppercase font-semibold">
                <tr>
                  <th className="px-2 py-2 border-b text-left">Ability</th>
                  <th className="px-2 py-2 border-b">Score</th>
                  <th className="px-2 py-2 border-b">Modifier</th>
                  <th className="px-2 py-2 border-b">Base</th>
                  <th className="px-2 py-2 border-b">Additional</th>
                </tr>
              </thead>
              <tbody>
                {abilityScores.abilityScores.map((ability) => (
                  <tr key={ability.abilityScoreId} className="">
                    <td className="px-2 py-1 border-b ">
                      <span className="flex items-center gap-1 text-sm uppercase">{ability.name}</span>
                    </td>
                    <td className="px-2 py-1 border-b text-sm">{ability.calculatedScore}</td>
                    <td className="px-2 py-1 border-b text-sm">
                      {ability.calculatedModifier != null
                        ? ability.calculatedModifier >= 0
                          ? `+${ability.calculatedModifier}`
                          : `${ability.calculatedModifier}`
                        : null}
                    </td>
                    <td className="px-2 py-1 border-b ">
                      <div className="flex flex-row items-center justify-center gap-2 ">
                        <Button
                          size="icon"
                          variant="outline"
                          onClick={() => {
                            /* TODO: decrease base score */
                            const newScore = ability.baseScore - 1;
                            console.log(`Decrease ${ability.name} base score to ${newScore}`);
                            updateBaseAbilityScore.mutate({ abilityScoreId: ability.abilityScoreId, value: newScore });
                          }}
                          className="h-8 w-8 p-0"
                          title={`Decrease ${ability.name} base score`}
                          aria-label={`Decrease ${ability.name} base score`}
                        >
                          <ChevronDown />
                        </Button>

                        <span className="text-xs py-1 w-10 h-8 border border-dashed rounded leading-6 text-center">{ability.baseScore}</span>

                        <Button
                          size="icon"
                          variant="outline"
                          onClick={() => {
                            /* TODO: increase base score */
                            const newScore = ability.baseScore + 1;
                            console.log(`Increase ${ability.name} base score to ${newScore}`);
                            updateBaseAbilityScore.mutate({ abilityScoreId: ability.abilityScoreId, value: newScore });
                          }}
                          className="h-8 w-8 p-0"
                          title={`Increase ${ability.name} base score`}
                          aria-label={`Increase ${ability.name} base score`}
                        >
                          <ChevronUp />
                        </Button>
                      </div>
                    </td>
                    <td className="px-2 py-1 border-b">
                      <div className="flex flex-row items-center justify-center gap-2 ">
                        <Button
                          size="icon"
                          variant="outline"
                          onClick={() => {
                            /* TODO: decrease base score */
                            const newScore = ability.additionalScore - 1;
                            console.log(`Decrease ${ability.name} additional score to ${newScore}`);
                            updateAdditionalAbilityScore.mutate({ abilityScoreId: ability.abilityScoreId, value: newScore });
                          }}
                          className="h-8 w-8 p-0"
                          title={`Decrease ${ability.name} additional score`}
                          aria-label={`Decrease ${ability.name} additional score`}
                        >
                          <ChevronDown />
                        </Button>

                        <span className="text-xs py-1 w-10 h-8 border border-dashed rounded leading-6 text-center">{ability.additionalScore}</span>

                        <Button
                          size="icon"
                          variant="outline"
                          onClick={() => {
                            /* TODO: increase base score */
                            const newScore = ability.additionalScore + 1;
                            console.log(`Increase ${ability.name} additional score to ${newScore}`);
                            updateAdditionalAbilityScore.mutate({ abilityScoreId: ability.abilityScoreId, value: newScore });
                          }}
                          className="h-8 w-8 p-0"
                          title={`Increase ${ability.name} additional score`}
                          aria-label={`Increase ${ability.name} additional score`}
                        >
                          <ChevronUp />
                        </Button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </>
  );
}

function SavingThrowsComponent({ characterId }: { characterId: string }) {
  const { data: savingThrowsData, isLoading, error } = useSavingThrows(characterId);

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div className="flex flex-row items-center justify-between mb-2 border border-dashed rounded p-4">
      {savingThrowsData && (
        <div className="overflow-x-auto min-w-full">
          <h2 className="text-xs font-semibold uppercase text-center bg-accent p-2 rounded leading-6 tracking-widest">Saving Throws</h2>
          <Separator className="my-4" />

          <table className="min-w-full text-center">
            <thead className="text-xs uppercase font-semibold">
              <tr>
                <th className="px-2 py-2 border-b text-left">Saving Throw</th>
                <th className="px-2 py-2 border-b">Bonus</th>
                <th className="px-2 py-2 border-b">Ability Modifier</th>
                <th className="px-2 py-2 border-b">Additional Bonus</th>
              </tr>
            </thead>
            <tbody>
              {savingThrowsData.savingThrows.map((save) => (
                <tr key={save.savingThrowId}>
                  <td className="px-2 py-1 border-b ">
                    <span className="flex items-center gap-1 text-sm uppercase">
                      {save.name} ({save.abilityScoreAbbreviation})
                    </span>
                  </td>

                  <td className="px-2 py-1 border-b text-sm">{save.calculatedBonus}</td>
                  <td className="px-2 py-1 border-b text-sm">{save.abilityScoreModifier}</td>
                  <td className="px-2 py-1 border-b text-sm">{save.additionalBonus}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

function SkillsComponent({ characterId }: { characterId: string }) {
  const { data: skillsData, isLoading, error } = useSkills(characterId);
  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;
  return (
    <div className="flex flex-row items-center justify-between mb-2 border border-dashed rounded p-4">
      {skillsData && (
        <div className="overflow-x-auto min-w-full">
          <h2 className="text-xs font-semibold uppercase text-center bg-accent p-2 rounded leading-6 tracking-widest">Skills</h2>
          <Separator className="my-4" />

          <table className="min-w-full text-center">
            <thead className="text-xs uppercase font-semibold">
              <tr>
                <th className="px-2 py-2 border-b text-left">Skill</th>
                <th className="px-2 py-2 border-b">Bonus</th>
                {/* <th className="px-2 py-2 border-b">Ability Modifier</th> */}
                {/* <th className="px-2 py-2 border-b">Additional Bonus</th> */}
              </tr>
            </thead>
            <tbody>
              {skillsData.skills.map((skill) => (
                <tr key={skill.skillId}>
                  <td className="px-2 py-1 border-b text-left">
                    <span className="flex items-center gap-1 text-sm uppercase">
                      {skill.name}
                      <span className="text-muted-foreground text-xxs">({skill.abilityScoreAbbreviation})</span>
                    </span>
                  </td>

                  <td className="px-2 py-1 border-b">
                    <div className="flex flex-row items-center justify-center gap-2 ">
                      <span className="text-xs py-1 w-10 h-8 border border-dashed rounded leading-6 text-center">{skill.calculatedBonus}</span>
                    </div>
                  </td>
                  {/* <td className="px-2 py-1 border-b text-sm">{skill.abilityScoreModifier}</td> */}
                  {/* <td className="px-2 py-1 border-b text-sm">{skill.additionalBonus}</td> */}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

export default function CharactersDetailsPage() {
  const { id } = useParams<{ id: string }>();

  if (!id) return <div>Character ID is required.</div>;
  // TODO: fetch character details (name, etc.)

  return (
    <>
      <div className="space-y-2">
        <h2 className="text-xl font-semibold">Character Details</h2>
        <p className="text-sm">
          This is a test page is for viewing and editing character details through the API. This is by no means representative of the final UI.
        </p>
        <p className="text-sm">TODO: ordering</p>
      </div>
      <div className="border border-dashed rounded p-4 my-4 text-sm">
        <p>
          <span className="font-semibold">ID:</span> {id}
        </p>
        <hr className="my-2" />
        <p>
          <span className="font-semibold">Name:</span> character.name
        </p>
        <hr className="my-2" />
        <p>
          <span className="font-semibold">Level:</span> character.level
        </p>
        <hr className="my-2" />
        <p>
          <span className="font-semibold">Build:</span> character.build
        </p>
      </div>
      <hr className="my-4" />
      <AbilitiesComponent id={id} />
      <hr className="my-4" />
      <SavingThrowsComponent characterId={id} />
      <hr className="my-4" />
      <SkillsComponent characterId={id} />
    </>
  );
}
