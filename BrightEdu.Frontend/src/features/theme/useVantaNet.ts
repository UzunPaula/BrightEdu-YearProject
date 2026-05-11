import { useEffect, useRef } from "react";

declare const VANTA: {
  NET: (opts: Record<string, unknown>) => { destroy: () => void };
};

export function useVantaNet(elementId: string, theme: "light" | "dark") {
  const effectRef = useRef<{ destroy: () => void } | null>(null);

  useEffect(() => {
    if (typeof VANTA === "undefined") return;

    effectRef.current?.destroy();

    effectRef.current = VANTA.NET({
      el: `#${elementId}`,
      mouseControls: true,
      touchControls: true,
      gyroControls: false,
      minHeight: 200,
      minWidth: 200,
      scale: 1.0,
      scaleMobile: 1.0,
      color: 0xf76c9d,
      backgroundColor: theme === "dark" ? 0x0f0720 : 0xfff6f9,
      points: 10.0,
      maxDistance: 22.0,
      spacing: 16.0,
      showDots: true,
    });

    return () => {
      effectRef.current?.destroy();
      effectRef.current = null;
    };
  }, [elementId, theme]);
}
