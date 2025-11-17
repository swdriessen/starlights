import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";

// Starlights color palettes configuration
const starlightsColors = [
  { name: "purple", label: "Purple", prefix: "starlights-" },
  { name: "red", label: "Red", prefix: "starlights-" },
  { name: "pink", label: "Pink", prefix: "starlights-" },
  { name: "cyan", label: "Cyan", prefix: "starlights-" },
  { name: "indigo", label: "Indigo", prefix: "starlights-" },
  { name: "green", label: "Green", prefix: "starlights-" },
  { name: "yellow", label: "Yellow", prefix: "starlights-" },
];

const shades = [50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 950, 1000];

const specialColors = [
  { name: "development", label: "Development" },
  { name: "debug", label: "Debug" },
];
const specialColors2 = [
  { name: "primary", label: "Primary" },
  { name: "secondary", label: "Secondary" },
  { name: "tertiary", label: "Tertiary" },
  { name: "quaternary", label: "Quaternary" },
  { name: "success", label: "Success" },
  { name: "warning", label: "Warning" },
  { name: "error", label: "Error" },
  { name: "info", label: "Info" },
];

// Tailwind CSS v4 default colors for comparison
const tailwindColors = [
  { name: "slate", label: "Slate" },
  { name: "gray", label: "Gray" },
  { name: "zinc", label: "Zinc" },
  { name: "red", label: "Red" },
  { name: "orange", label: "Orange" },
  { name: "yellow", label: "Yellow" },
  { name: "green", label: "Green" },
  { name: "blue", label: "Blue" },
  { name: "indigo", label: "Indigo" },
  { name: "purple", label: "Purple" },
  { name: "pink", label: "Pink" },
];

const tailwindShades = [50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 950];

// Hardcoded color class maps for Starlights colors
const starlightsColorClassMap: Record<string, Record<number, string>> = {
  "starlights-purple": {
    50: "bg-starlights-purple-50",
    100: "bg-starlights-purple-100",
    200: "bg-starlights-purple-200",
    300: "bg-starlights-purple-300",
    400: "bg-starlights-purple-400",
    500: "bg-starlights-purple-500",
    600: "bg-starlights-purple-600",
    700: "bg-starlights-purple-700",
    800: "bg-starlights-purple-800",
    900: "bg-starlights-purple-900",
    950: "bg-starlights-purple-950",
    1000: "bg-starlights-purple-1000",
  },
  "starlights-red": {
    50: "bg-starlights-red-50",
    100: "bg-starlights-red-100",
    200: "bg-starlights-red-200",
    300: "bg-starlights-red-300",
    400: "bg-starlights-red-400",
    500: "bg-starlights-red-500",
    600: "bg-starlights-red-600",
    700: "bg-starlights-red-700",
    800: "bg-starlights-red-800",
    900: "bg-starlights-red-900",
    950: "bg-starlights-red-950",
    1000: "bg-starlights-red-1000",
  },
  "starlights-pink": {
    50: "bg-starlights-pink-50",
    100: "bg-starlights-pink-100",
    200: "bg-starlights-pink-200",
    300: "bg-starlights-pink-300",
    400: "bg-starlights-pink-400",
    500: "bg-starlights-pink-500",
    600: "bg-starlights-pink-600",
    700: "bg-starlights-pink-700",
    800: "bg-starlights-pink-800",
    900: "bg-starlights-pink-900",
    950: "bg-starlights-pink-950",
    1000: "bg-starlights-pink-1000",
  },
  "starlights-cyan": {
    50: "bg-starlights-cyan-50",
    100: "bg-starlights-cyan-100",
    200: "bg-starlights-cyan-200",
    300: "bg-starlights-cyan-300",
    400: "bg-starlights-cyan-400",
    500: "bg-starlights-cyan-500",
    600: "bg-starlights-cyan-600",
    700: "bg-starlights-cyan-700",
    800: "bg-starlights-cyan-800",
    900: "bg-starlights-cyan-900",
    950: "bg-starlights-cyan-950",
    1000: "bg-starlights-cyan-1000",
  },
  "starlights-indigo": {
    50: "bg-starlights-indigo-50",
    100: "bg-starlights-indigo-100",
    200: "bg-starlights-indigo-200",
    300: "bg-starlights-indigo-300",
    400: "bg-starlights-indigo-400",
    500: "bg-starlights-indigo-500",
    600: "bg-starlights-indigo-600",
    700: "bg-starlights-indigo-700",
    800: "bg-starlights-indigo-800",
    900: "bg-starlights-indigo-900",
    950: "bg-starlights-indigo-950",
    1000: "bg-starlights-indigo-1000",
  },
  "starlights-green": {
    50: "bg-starlights-green-50",
    100: "bg-starlights-green-100",
    200: "bg-starlights-green-200",
    300: "bg-starlights-green-300",
    400: "bg-starlights-green-400",
    500: "bg-starlights-green-500",
    600: "bg-starlights-green-600",
    700: "bg-starlights-green-700",
    800: "bg-starlights-green-800",
    900: "bg-starlights-green-900",
    950: "bg-starlights-green-950",
    1000: "bg-starlights-green-1000",
  },
  "starlights-yellow": {
    50: "bg-starlights-yellow-50",
    100: "bg-starlights-yellow-100",
    200: "bg-starlights-yellow-200",
    300: "bg-starlights-yellow-300",
    400: "bg-starlights-yellow-400",
    500: "bg-starlights-yellow-500",
    600: "bg-starlights-yellow-600",
    700: "bg-starlights-yellow-700",
    800: "bg-starlights-yellow-800",
    900: "bg-starlights-yellow-900",
    950: "bg-starlights-yellow-950",
    1000: "bg-starlights-yellow-1000",
  },
};

// Hardcoded special color classes
const specialColorClassMap: Record<string, string> = {
  development: "bg-starlights-development",
  debug: "bg-starlights-debug",
};

const specialColorClassMap2: Record<string, string> = {
  primary: "bg-primary",
  secondary: "bg-secondary",
  tertiary: "bg-tertiary",
  quaternary: "bg-quaternary",
};

// Hardcoded Tailwind color classes
const tailwindColorClassMap: Record<string, Record<number, string>> = {
  slate: {
    50: "bg-slate-50",
    100: "bg-slate-100",
    200: "bg-slate-200",
    300: "bg-slate-300",
    400: "bg-slate-400",
    500: "bg-slate-500",
    600: "bg-slate-600",
    700: "bg-slate-700",
    800: "bg-slate-800",
    900: "bg-slate-900",
    950: "bg-slate-950",
  },
  gray: {
    50: "bg-gray-50",
    100: "bg-gray-100",
    200: "bg-gray-200",
    300: "bg-gray-300",
    400: "bg-gray-400",
    500: "bg-gray-500",
    600: "bg-gray-600",
    700: "bg-gray-700",
    800: "bg-gray-800",
    900: "bg-gray-900",
    950: "bg-gray-950",
  },
  zinc: {
    50: "bg-zinc-50",
    100: "bg-zinc-100",
    200: "bg-zinc-200",
    300: "bg-zinc-300",
    400: "bg-zinc-400",
    500: "bg-zinc-500",
    600: "bg-zinc-600",
    700: "bg-zinc-700",
    800: "bg-zinc-800",
    900: "bg-zinc-900",
    950: "bg-zinc-950",
  },
  red: {
    50: "bg-red-50",
    100: "bg-red-100",
    200: "bg-red-200",
    300: "bg-red-300",
    400: "bg-red-400",
    500: "bg-red-500",
    600: "bg-red-600",
    700: "bg-red-700",
    800: "bg-red-800",
    900: "bg-red-900",
    950: "bg-red-950",
  },
  orange: {
    50: "bg-orange-50",
    100: "bg-orange-100",
    200: "bg-orange-200",
    300: "bg-orange-300",
    400: "bg-orange-400",
    500: "bg-orange-500",
    600: "bg-orange-600",
    700: "bg-orange-700",
    800: "bg-orange-800",
    900: "bg-orange-900",
    950: "bg-orange-950",
  },
  yellow: {
    50: "bg-yellow-50",
    100: "bg-yellow-100",
    200: "bg-yellow-200",
    300: "bg-yellow-300",
    400: "bg-yellow-400",
    500: "bg-yellow-500",
    600: "bg-yellow-600",
    700: "bg-yellow-700",
    800: "bg-yellow-800",
    900: "bg-yellow-900",
    950: "bg-yellow-950",
  },
  green: {
    50: "bg-green-50",
    100: "bg-green-100",
    200: "bg-green-200",
    300: "bg-green-300",
    400: "bg-green-400",
    500: "bg-green-500",
    600: "bg-green-600",
    700: "bg-green-700",
    800: "bg-green-800",
    900: "bg-green-900",
    950: "bg-green-950",
  },
  blue: {
    50: "bg-blue-50",
    100: "bg-blue-100",
    200: "bg-blue-200",
    300: "bg-blue-300",
    400: "bg-blue-400",
    500: "bg-blue-500",
    600: "bg-blue-600",
    700: "bg-blue-700",
    800: "bg-blue-800",
    900: "bg-blue-900",
    950: "bg-blue-950",
  },
  indigo: {
    50: "bg-indigo-50",
    100: "bg-indigo-100",
    200: "bg-indigo-200",
    300: "bg-indigo-300",
    400: "bg-indigo-400",
    500: "bg-indigo-500",
    600: "bg-indigo-600",
    700: "bg-indigo-700",
    800: "bg-indigo-800",
    900: "bg-indigo-900",
    950: "bg-indigo-950",
  },
  purple: {
    50: "bg-purple-50",
    100: "bg-purple-100",
    200: "bg-purple-200",
    300: "bg-purple-300",
    400: "bg-purple-400",
    500: "bg-purple-500",
    600: "bg-purple-600",
    700: "bg-purple-700",
    800: "bg-purple-800",
    900: "bg-purple-900",
    950: "bg-purple-950",
  },
  pink: {
    50: "bg-pink-50",
    100: "bg-pink-100",
    200: "bg-pink-200",
    300: "bg-pink-300",
    400: "bg-pink-400",
    500: "bg-pink-500",
    600: "bg-pink-600",
    700: "bg-pink-700",
    800: "bg-pink-800",
    900: "bg-pink-900",
    950: "bg-pink-950",
  },
};

// Helper functions
const getStarlightsBgColorClass = (colorName: string, prefix: string, shade: number) => {
  const key = prefix ? `${prefix}${colorName}` : colorName;
  return starlightsColorClassMap[key]?.[shade] || "";
};

const getTailwindBgColorClass = (color: string, shade: number) => {
  return tailwindColorClassMap[color]?.[shade] || "";
};

export function DevelopmentPage() {
  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Starlights Colors</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full border-collapse">
              <thead>
                <tr>
                  <th className="text-left p-2 text-sm font-semibold border-b">Color</th>
                  {shades.map((shade) => (
                    <th key={shade} className="p-1 text-xs font-mono border-b text-center min-w-[40px]">
                      {shade}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {starlightsColors.map((color) => (
                  <tr key={color.name} className="border-b last:border-b-0">
                    <td className="p-2 text-sm font-medium">{color.label}</td>
                    {shades.map((shade) => (
                      <td key={shade} className="p-1">
                        <div
                          className={`w-full h-8 rounded border hover:scale-200 transition-transform cursor-pointer ${getStarlightsBgColorClass(
                            color.name,
                            color.prefix,
                            shade
                          )}`}
                          title={`${color.prefix}${color.name}-${shade}`}
                        />
                      </td>
                    ))}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Special Colors</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full border-collapse">
              <thead>
                <tr>
                  <th className="text-left p-2 text-sm font-semibold border-b">Color</th>
                  <th className="p-2 text-sm font-semibold border-b text-center">Preview</th>
                  <th className="text-left p-2 text-sm font-semibold border-b">Class Name</th>
                </tr>
              </thead>
              <tbody>
                {specialColors.map((color) => (
                  <tr key={color.name} className="border-b">
                    <td className="p-2 text-sm font-medium">{color.label}</td>
                    <td className="p-2">
                      <div className={`w-full h-8 rounded border ${specialColorClassMap[color.name]}`} title={`starlights-${color.name}`} />
                    </td>
                    <td className="p-2 text-xs font-mono text-muted-foreground">starlights-{color.name}</td>
                  </tr>
                ))}
                {specialColors2.map((color) => (
                  <tr key={color.name} className="border-b last:border-b-0">
                    <td className="p-2 text-sm font-medium">{color.label}</td>
                    <td className="p-2">
                      <div className={`w-full h-8 rounded border ${specialColorClassMap2[color.name]}`} title={`${color.name}`} />
                    </td>
                    <td className="p-2 text-xs font-mono text-muted-foreground">{color.name}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Tailwind CSS v4 Colors</CardTitle>
          <CardDescription>Comparison with default Tailwind colors</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full border-collapse">
              <thead>
                <tr>
                  <th className="text-left p-2 text-sm font-semibold border-b">Color</th>
                  {tailwindShades.map((shade) => (
                    <th key={shade} className="p-1 text-xs font-mono border-b text-center min-w-[40px]">
                      {shade}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {tailwindColors.map((color) => (
                  <tr key={color.name} className="border-b last:border-b-0">
                    <td className="p-2 text-sm font-medium">{color.label}</td>
                    {tailwindShades.map((shade) => (
                      <td key={shade} className="p-1">
                        <div
                          className={`w-full h-8 rounded border hover:scale-200 transition-transform cursor-pointer ${getTailwindBgColorClass(
                            color.name,
                            shade
                          )}`}
                          title={`${color.name}-${shade}`}
                        />
                      </td>
                    ))}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
