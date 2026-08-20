import express from "express";
import cors from "cors";
import { DatabaseSync } from "node:sqlite";

import translationsRouter from "./routes/translations.js";
// import usersRouter from "./routes/users.js";

const app = express();
const db = new DatabaseSync("database.db");

process.loadEnvFile();

const adminName = process.env.LOGIN;
const adminKey = process.env.PASS;

db.exec(`
  CREATE TABLE IF NOT EXISTS admins(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  key TEXT NOT NULL
  );
`);

const exists = db
  .prepare(
    `
  SELECT 1
  FROM admins
  WHERE name = ?
`,
  )
  .get(adminName);

if (!exists) {
  db.prepare(
    `
    INSERT INTO admins (name, key)
    VALUES (?, ?)
  `,
  ).run(adminName, adminKey);

  console.log("Admin criado.");
}

app.use(
  cors({
    origin: [
      "http://localhost:5173",
      "https://doki-doki-translation-club.vercel.app",
      "https://isneiki.github.io",
    ],
  }),
);
app.use(express.json());

// routers
app.use("/translations", translationsRouter);
// app.use("/users", usersRouter);

app.get("/", (req, res) => {
  res.json({
    message: "Server working.",
  });
});

app.listen(3000, () => {
  console.log("Running");
});

// deploy:
// app.listen(Number(process.env.PORT), process.env.HOST, () => {
//   console.log("Running");
// });
