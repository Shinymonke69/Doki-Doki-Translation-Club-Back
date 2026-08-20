import express from "express";
import { DatabaseSync } from "node:sqlite";

const db = new DatabaseSync("database.db");

const router = express.Router();

db.exec(`
  CREATE TABLE IF NOT EXISTS translations(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    description TEXT NOT NULL,
    banner TEXT NOT NULL,
    image TEXT NOT NULL,
    linkPC TEXT NOT NULL,
    linkMobile TEXT NOT NULL
  );
`);

const checkAuth = (authHeader) => {
  if (!authHeader || !authHeader.startsWith("Bearer ")) {
    return false;
  }

  const adminKey = authHeader.split(" ")[1];

  const auth = db.prepare(`
    SELECT 1
    FROM admins
    WHERE key = ?
  `);

  return !!auth.get(adminKey);
};

router.post("/new", (req, res) => {
  const { name, description, banner, img, linkPC, linkMobile } = req.body;

  const authHeader = req.headers.authorization;

  if (!checkAuth(authHeader)) {
    return res.status(401).json({
      message: "Chave inválida ou inexistente.",
    });
  }

  const stmt = db.prepare(`
    INSERT INTO translations (name, description, banner, image, linkPC, linkMobile) VALUES (?, ?, ?, ?, ?, ?);  
  `);

  stmt.run(name, description, banner, img, linkPC, linkMobile);

  res.send({
    message: `Mod "${name}" adicionado à database, confira a aba de traduções para garantir que não há erros visuais.`,
  });
});

router.post("/remove", (req, res) => {
  const { id } = req.body;

  const authHeader = req.headers.authorization;

  if (!checkAuth(authHeader)) {
    return res.status(401).json({
      message: "Chave inválida ou inexistente.",
    });
  }

  const stmt = db.prepare(`
    DELETE FROM translations WHERE id = ?;  
  `);

  const result = stmt.run(id);

  res.json({
    success: true,
    changes: result.changes,
  });
});

router.post("/edit/:id", (req, res) => {
  const { name, description, banner, img, linkPC, linkMobile } = req.body;

  const authHeader = req.headers.authorization;

  if (!checkAuth(authHeader)) {
    return res.status(401).json({
      message: "Chave inválida ou inexistente.",
    });
  }

  const stmt = db.prepare(`
    UPDATE translations 
    SET name = ?,
    description = ?,
    banner = ?,
    image = ?,
    linkPC = ?,
    linkMobile = ? 
    WHERE id = ?;
  `);

  const result = stmt.run(
    name,
    description,
    banner,
    img,
    linkPC,
    linkMobile,
    Number(req.params.id),
  );

  res.json({
    success: true,
    changes: result.changes,
    message: "Mudanças feitas!",
  });
});

router.get("/all", (req, res) => {
  const query = db.prepare(`
    SELECT * FROM translations
    ORDER BY id;  
  `);

  res.json({
    translations: query.all(),
  });
});

router.get("/popular", (req, res) => {
  const query = db.prepare(`
    SELECT * FROM translations
    ORDER BY id
    LIMIT 3;
  `);

  res.json({
    translations: query.all(),
  });
});

router.get("/:id", (req, res) => {
  const query = db.prepare(`SELECT 1 FROM translations WHERE id = ?`);

  const translation = query.get(Number(req.params.id));

  res.send({ translation: translation });
});

export default router;
