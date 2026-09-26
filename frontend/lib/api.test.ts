import { describe, expect, it } from "vitest";
import { label, money } from "./api";

describe("presentation utilities", () => {
  it("formats INR without floating currency ambiguity", () => {
    expect(money(1045000)).toContain("10,45,000");
  });

  it("turns controlled enum values into readable labels", () => {
    expect(label("NEEDS_INFORMATION")).toBe("Needs information");
  });
});
