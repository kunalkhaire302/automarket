const folders: Record<string, string> = {
  "Maruti Suzuki": "Maruti_Suzuki", Hyundai: "Hyundai", Tata: "Tata", Honda: "Honda", Kia: "Kia",
  Toyota: "Toyota", Mahindra: "Mahindra", Renault: "Renault", Nissan: "Nissan", Volkswagen: "Volkswagen",
  Skoda: "Skoda", MG: "MG", Jeep: "Jeep", Ford: "Ford"
};

// Wagon R is the only seeded model absent from the supplied pack; it intentionally retains the neutral fallback.
const unavailable = new Set(["Maruti Suzuki/Wagon R"]);

export function demoPhotoFor(brand: string, model: string) {
  const folder = folders[brand];
  if (!folder || unavailable.has(`${brand}/${model}`)) return undefined;
  const filename = model.replaceAll(" ", "_").replaceAll("-", "_");
  return { url: `/demo-cars/photos/${folder}/${filename}.jpg` };
}
