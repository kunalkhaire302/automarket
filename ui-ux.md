# AutoMarket — UI/UX Specification

## 1. Product UX Goal

Create a premium, trustworthy used-car marketplace experience that is fast to understand, easy to search, simple to book, and clear about vehicle information and AI-generated assistance.

The product may take inspiration from modern marketplace patterns, including Cars24, Stripe, Linear, Vercel, and polished commerce platforms, but must use its own branding, content, imagery, and visual identity.

## 2. UX Principles

### Clarity

Show the most important information first: price, vehicle identity, year, mileage, fuel, transmission, location, availability, and primary action.

### Progressive disclosure

Reveal advanced filters and secondary information without overwhelming first-time users.

### Confidence

Use predictable interaction patterns, clear states, transparent pricing/estimates, and strong confirmation screens.

### Efficiency

Reduce unnecessary clicks in search, seller submission, appointments, and booking.

### Accessibility

Meet WCAG-oriented practices:

- keyboard navigation;
- semantic HTML;
- visible focus states;
- sufficient contrast;
- labeled controls;
- alt text for meaningful images;
- reduced-motion support where appropriate.

## 3. Design System

### Typography

Use one primary UI family and a restrained hierarchy:

```text
Display
H1
H2
H3
Body Large
Body
Caption
Label
```

Do not use excessive font weights or decorative styles.

### Spacing

Use a consistent 4/8-point spacing rhythm or another documented scale.

### Radius

Use a consistent radius system for:

- cards;
- inputs;
- buttons;
- modals;
- image containers.

### Shadows

Use restrained shadows to establish layers, not as decoration.

## 4. Responsive Breakpoints

Define a documented system such as:

```text
Mobile
Tablet
Desktop
Large Desktop
```

All core workflows must work at touch-friendly widths.

## 5. Global Navigation

Desktop:

- logo/brand;
- Buy Cars;
- Sell Car;
- favourites/compare where implemented;
- profile/auth actions.

Mobile:

- compact header;
- prominent search/CTA;
- menu or bottom navigation where justified.

## 6. Home Page UX

Recommended hierarchy:

```text
Header
 ↓
Hero + Search
 ↓
Featured Cars
 ↓
Popular Brands / Categories
 ↓
How It Works
 ↓
Trust / Inspection / Process information
 ↓
FAQ
 ↓
Footer
```

The hero should communicate what the platform does without relying on unexplained marketing claims.

## 7. Buy Cars UX

### Search

Search should remain visible and easy to edit.

### Filters

Desktop:

- side filter panel or top filter bar.

Mobile:

- filter drawer/modal;
- sticky Apply action.

### Sorting

Examples:

- Relevance;
- Price low to high;
- Price high to low;
- Newest listed;
- Mileage low to high.

Avoid hidden sorting logic.

## 8. Car Card

Minimum information:

- primary image;
- brand/model/variant;
- price;
- year;
- mileage;
- fuel;
- transmission;
- city;
- favourite;
- compare.

The card must have a clear clickable area and accessible labels.

## 9. Car Detail UX

Recommended hierarchy:

```text
Gallery
 ↓
Vehicle title + price + location
 ↓
Key specifications
 ↓
Primary actions
 ↓
Overview
 ↓
Features
 ↓
Inspection / condition information
 ↓
AI decision support
 ↓
Similar cars
```

Primary actions must remain discoverable without competing with the vehicle information.

## 10. AI UX

AI output must look like a decision-support component, not a guaranteed truth indicator.

Example:

```text
AI recommendation
82% confidence

Why this car:
• Fits your budget
• Automatic transmission matches your preference
• Similar to cars you saved

Note: AI suggestions are estimates and may be incomplete.
```

Do not use misleading labels such as “perfect car” or “guaranteed best deal.”

## 11. Sell Car UX

Use a multi-step form.

```text
1. Vehicle basics
2. Usage
3. Condition
4. Photos
5. Price/contact
6. Review & submit
```

UX requirements:

- show progress;
- preserve entered data when moving between steps;
- inline validation;
- clear photo requirements;
- summary before submission;
- explicit final confirmation.

## 12. Image Upload UX

Show:

- required views;
- upload progress;
- thumbnail preview;
- remove/reorder;
- invalid-file messages;
- image quality feedback.

AI image-condition results must be clearly separated from verified inspection data.

## 13. Appointment UX

Steps:

```text
Choose car
 ↓
Choose appointment type
 ↓
Choose date
 ↓
Choose time slot
 ↓
Confirm details
 ↓
Appointment confirmation
```

Unavailable slots must not appear selectable.

## 14. Booking UX

A booking review screen should show:

- car;
- appointment;
- location;
- date/time;
- amount/fees when applicable;
- cancellation policy if applicable;
- final action.

After creation show a durable booking reference.

## 15. Profile UX

Sections:

- Profile;
- Saved cars;
- Compare;
- Appointments;
- Bookings;
- Seller submissions;
- Notifications;
- Security/preferences.

## 16. UI State Model

Every major asynchronous module must implement:

```text
Initial
 ↓
Loading
 ├→ Success
 ├→ Empty
 ├→ Error → Retry
 └→ Partial / degraded where applicable
```

For AI:

```text
Request
 ↓
Loading
 ↓
Success / Low confidence / Unavailable
```

## 17. Forms

Rules:

- label every field;
- show constraints before or at entry;
- validate on interaction without being noisy;
- preserve input on recoverable errors;
- prevent duplicate submission;
- show server errors at the relevant field and form level.

## 18. Motion

Motion should communicate:

- navigation;
- state changes;
- loading;
- confirmation.

Do not use animation that delays or blocks core tasks.

Respect `prefers-reduced-motion`.

## 19. Error UX

Good:

> We couldn't load the cars right now. Retry.

Bad:

> Error 500 null undefined.

Never expose internal stack traces to users.

## 20. Empty States

Examples:

### No search results

Explain why the result set is empty and provide a clear action such as clearing a filter.

### No favourites

Explain the value of saving cars and provide a Buy Cars CTA.

### No bookings

Provide a path back to browsing.

## 21. Accessibility Acceptance Criteria

- tab order is logical;
- keyboard focus visible;
- buttons have accessible names;
- dialogs trap focus appropriately;
- images have useful alt text;
- forms associate labels/errors correctly;
- color is not the only indicator of state;
- text remains readable at zoomed layouts.

## 22. Performance UX

- optimize vehicle images;
- use responsive image loading;
- lazy-load non-critical media;
- minimize client JavaScript where possible;
- show skeletons for known layout regions;
- avoid layout shift.

## 23. UI Component Library

Recommended categories:

```text
Layout
Typography
Buttons
Inputs
Selects
Checkboxes
Modals
Drawers
Toasts
Skeletons
Car Cards
Filter Panels
Gallery
Calendar
Status Badges
Data Tables
Charts
AI Cards
```

## 24. UX Acceptance Criteria

- all primary flows work on desktop and mobile;
- every important API action has visible loading/error/success states;
- critical CTAs are obvious;
- forms preserve user progress;
- AI limitations are visible;
- no screen depends on unexplained client-only state;
- navigation is consistent throughout the product.
