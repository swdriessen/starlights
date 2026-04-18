import DescriptionProseSection from "@/components/description-section";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { cn } from "@/lib/utils";

function DescriptionPanel({ id, className }: { id: string; className?: string }) {
  return (
    <div>
      <DescriptionProseSection>
        {/* <div className="h-20 p-4 overflow-hidden">
        Lorem ipsum dolor sit amet consectetur adipisicing elit. Odit recusandae neque, harum vero a asperiores sed impedit fugiat natus ipsa est, illum
        reiciendis, officia molestias. Dignissimos placeat ut nobis doloremque voluptas vitae ab vel, et odio ipsum rerum, ex aperiam velit facilis itaque
        consequatur, eum magnam quisquam porro! Laboriosam, pariatur quidem quae accusantium quis in rerum expedita impedit rem autem dignissimos reprehenderit
        maiores illo! Cupiditate consequatur maxime, similique sint cum adipisci nostrum soluta, itaque, totam iure unde enim obcaecati accusantium quasi
        nesciunt praesentium illum. Tempore aut qui ipsum consequuntur omnis, atque eveniet error assumenda incidunt sint nihil a necessitatibus quibusdam!
      </div> */}
        <ScrollArea className="pe-4">
          <div
            className={cn(
              "max-h-[calc(100svh-calc(1*var(--navigation-height)))]! h-[calc(100svh-calc(1*var(--navigation-height)))]! element-description",
              className
            )}
          >
            {/* <h1 className="">Arcane Archer</h1> */}
            <h2 className="">Arcane Archer</h2>
            <p>
              An Arcane Archer studies a unique elven method of archery that weaves magic into attacks to produce supernatural effects. Arcane Archers are some
              of the most elite warriors among the elves. They stand watch over the fringes of elven domains, keeping a keen eye out for trespassers and using
              magic—infused arrows to defeat monsters and invaders before they can reach elven settlements. Over the centuries, the methods of these elf archers
              have been learned by members of other races who can also balance arcane aptitude with archery.
            </p>
            <h4>Arcana Archer Lore</h4>
            <p>
              At 3rd level, you learn magical theory or some of the secrets of nature—typical for practitioners of this elven martial tradition. You choose to
              gain proficiency in either the Arcana or the Nature skill, and you choose to learn either the prestidigitation or the druidcraft cantrip.
            </p>
            <h4>Arcane Shot</h4>
            <p className="">
              At 3rd level, you learn to unleash special magical effects with some of your shots. When you gain this feature, you learn two Arcane Shot options
              of your choice. Once per turn when you fire an arrow from a shortbow or longbow as part of the Attack action, you can apply one of your Arcane
              Shot options to that arrow. You decide to use the option when the arrow hits a creature, unless the option doesn’t involve an attack roll. You
              have two uses of this ability, and you regain all expended uses of it when you finish a short or long rest. You gain an additional Arcane Shot
              option of your choice when you reach certain levels in this class: 7th, 10th, 15th, and 18th level. Each option also improves when you become an
              18th—level fighter.
            </p>
            <h4>Magic Arrow</h4>
            <p>
              At 7th level, you gain the ability to infuse arrows with magic. Whenever you fire a nonmagical arrow from a shortbow or longbow, you can make it
              magical for the purpose of overcoming resistance and immunity to nonmagical attacks and damage. The magic fades from the arrow immediately after
              it hits or misses its target.
            </p>
            <h4>Curving Shot</h4>
            <p>
              At 7th level, you learn how to direct an errant arrow toward a new target. When you make an attack roll with a magic arrow and miss, you can use a
              bonus action to reroll the attack roll against a different target within 60 feet of the original target.
            </p>
            <h4>Ever-Ready Shot</h4>
            <p>
              Starting at 15th level, your magical archery is available whenever battle starts. If you roll initiative and have no uses of Arcane Shot
              remaining, you regain one use of it.
            </p>
            <Separator />
            <p className="text-foreground/40 text-xs">ID: {id}</p>
            <div className="h-3"></div>
          </div>
        </ScrollArea>
      </DescriptionProseSection>
    </div>
  );
}

export default DescriptionPanel;
