import { createLightTheme, createDarkTheme } from "@fluentui/react-components";
import type { BrandVariants, Theme } from "@fluentui/react-components";

const brand: BrandVariants = {
  10: "#020305", 20: "#111723", 30: "#16233A", 40: "#192C4B",
  50: "#1B355D", 60: "#1C3F70", 70: "#1B4983", 80: "#1B5497",
  90: "#215FA6", 100: "#3B6DB0", 110: "#537BB9", 120: "#6989C2",
  130: "#7E98CB", 140: "#92A7D4", 150: "#A6B6DD", 160: "#BAC5E6",
};

export const lightTheme: Theme = createLightTheme(brand);
export const darkTheme: Theme = createDarkTheme(brand);