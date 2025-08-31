import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { Toaster } from "@/components/ui/sonner";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { ThemeProvider } from "./components/theme-provider.tsx";
import { RouterProvider, createBrowserRouter } from "react-router-dom";
import LandingPage from "./pages/landing/Index.tsx";
import AboutPage from "./pages/about/Index.tsx";
import CharactersPage from "./pages/characters/Index.tsx";
import CharactersCreatePage from "./pages/characters/create/Index.tsx";
import CharactersDetailsPage from "./pages/characters/details/Index.tsx";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      staleTime: 5_000,
      refetchOnWindowFocus: false,
    },
  },
});

const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      { index: true, element: <LandingPage /> },
      { path: "about", element: <AboutPage /> },
      { path: "characters", element: <CharactersPage /> },
      { path: "characters/:id", element: <CharactersDetailsPage /> },
      { path: "characters/create", element: <CharactersCreatePage /> },
    ],
  },
]);

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <ThemeProvider defaultTheme="system" storageKey="vite-ui-theme">
        <RouterProvider router={router} />
        <ReactQueryDevtools initialIsOpen={false} buttonPosition="bottom-left" />
        <Toaster />
      </ThemeProvider>
    </QueryClientProvider>
  </StrictMode>
);
