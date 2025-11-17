import { Link } from "react-router-dom";
import { Badge } from "@/components/ui/badge";
import ProseSection from "@/components/prose-section";

function LandingTile({
  title,
  description,
  url,
  image,
  size = "lg",
  tag = undefined,
  enabled = true,
}: {
  title: string;
  description: string;
  url: string;
  image?: string;
  size?: "sm" | "lg";
  tag?: string;
  enabled?: boolean;
}) {
  const isLarge = size === "lg";

  return (
    <>
      <Link to={url} className="block h-full relative overflow-hidden rounded-xl group border-4 border-double shadow-lg">
        <img
          src={image || "https://www.dndbeyond.com/attachments/12/424/flash-sale.jpg"}
          alt={title}
          className={`absolute inset-0 w-full h-full object-cover transition-transform duration-500 scale-100 group-hover:scale-105 ${
            enabled ? "" : "grayscale-100 group-hover:grayscale-0 "
          }`}
        />
        <div className="absolute inset-0 bg-gradient-to-tr from-black/40 group-hover:from-black/20 to-transparent" />
        <div
          className={`prose prose-neutral dark:prose-invert absolute text-white ${
            isLarge ? "left-2 right-2 bottom-2 sm:left-8 sm:right-8 sm:bottom-8" : "left-2 right-2 bottom-2 sm:left-4 sm:right-4 sm:bottom-4"
          }  `}
        >
          {isLarge ? (
            <>
              <h3 className="text-lg! sm:text-3xl! text-white mb-0">{title}</h3>
              <p className="text-sm sm:text-base">{description}</p>
            </>
          ) : (
            <>
              <h3 className="text-base sm:text-xl! text-white mb-0 mt-1">{title}</h3>
              <p className="text-xs sm:text-sm">{description}</p>
            </>
          )}
        </div>
        <div className={` absolute ${isLarge ? "left-2 right-2 top-2 sm:left-8 sm:right-8 sm:top-8" : "left-2 top-2 sm:left-4 sm:right-4 sm:top-4"}  `}>
          {tag === undefined ? null : (
            <>
              <Badge variant={"outline"} className="backdrop-blur text-white border-white/50">
                {tag}
              </Badge>
            </>
          )}
        </div>
      </Link>
    </>
  );
}

const tiles = [
  {
    title: "Character Builder — In Development",
    description: "A web-based character builder to craft and chronicle your adventurers — fast, and battle-ready.",
    url: "/characters",
    image: "/images/group.jpg",
    tag: "In Development",
    enabled: true,
  },
  {
    title: "Campaign Ledger",
    description: "Maintain the campaign ledger: plan quests, track NPCs and sessions, and steer your party through every chapter of the story.",
    url: "/campaigns",
    image: "images/drow.jpg",
    tag: "Planned Feature",
    enabled: false,
  },
  {
    title: "Compendium of Lore",
    description: "A searchable archive of spells, items, lore, and beasts — a quick reference for everything in your adventure.",
    url: "/compendium",
    image: "/images/compendium.jpeg",
    tag: "Planned Feature",
    enabled: false,
  },
];

export function LandingPage2() {
  return (
    <>
      <div className="grid grid-cols-1 sm:grid-cols-5 gap-4 h-140 sm:h-120">
        <div className="col-span-1 sm:col-span-3">
          <LandingTile
            title={tiles[0].title}
            description={tiles[0].description}
            url={tiles[0].url}
            image={tiles[0].image}
            enabled={tiles[0].enabled}
            tag={tiles[0].tag}
          />
        </div>

        <div className="col-span-1 sm:col-span-2 grid grid-rows-2 gap-4">
          <LandingTile
            title={tiles[1].title}
            description={tiles[1].description}
            url={tiles[1].url}
            size="sm"
            image={tiles[1].image}
            enabled={tiles[1].enabled}
            tag={tiles[1].tag}
          />
          <div className="grid grid-cols-1 gap-4">
            <LandingTile
              title={tiles[2].title}
              description={tiles[2].description}
              url={tiles[2].url}
              size="sm"
              image={tiles[2].image}
              enabled={tiles[2].enabled}
              tag={tiles[2].tag}
            />
          </div>
        </div>
      </div>

      <ProseSection className="my-12">
        <h1>About</h1>
        <p>
          Project Starlights aims to be a comprehensive platform for managing and enhancing your tabletop role-playing game experience. With a focus on
          user-friendly design and powerful features, it aims to streamline character creation, campaign management, and collaborative storytelling.
        </p>
      </ProseSection>
    </>
  );
}
