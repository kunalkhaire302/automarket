export type Car = {
  id: string; brand: string; model: string; variant: string; manufacturingYear: number;
  registrationYear: number; kilometers: number; ownershipCount: number; fuelType: string;
  transmission: string; bodyType: string; price: number; cityId: string; city: string;
  state: string; description: string; status: string;
  isFeatured: boolean; primaryImageUrl?: string | null; images?: { imageType: string; url: string; sortOrder: number; sourceUrl?: string | null; attribution?: string | null }[];
};
export type City = { id: string; name: string; state: string; latitude: number; longitude: number };
export type Center = { id: string; name: string; address: string; kind: string; latitude: number; longitude: number };
export type User = { id: string; name: string; email: string; phone: string; role: string; referralCode: string };
export type Appointment = { id: string; carId: string; serviceCenterId: string; startsAt: string; endsAt: string; appointmentType: string; status: string; notes: string };
export type Booking = { id: string; carId: string; appointmentId: string; bookingReference: string; status: string; amount: number; createdAt: string };
export type Submission = { id: string; carId: string; status: string; reviewNotes: string; submittedAt: string | null };
export type Notice = { id: string; title: string; message: string; deepLink: string; readAt: string | null; createdAt: string };
export type WalletTransaction = { id: string; points: number; type: string; status: string; eventKey: string; referenceId: string | null; createdAt: string; expiresAt: string | null };
export type Wallet = { balance: number; transactions: WalletTransaction[] };
export type Meta = { page: number; pageSize: number; totalItems: number; totalPages: number; hasNext: boolean };
export type Envelope<T> = { success: boolean; data: T; meta?: Meta; error?: { code: string; message: string; fields?: Record<string, string> }; requestId: string };
