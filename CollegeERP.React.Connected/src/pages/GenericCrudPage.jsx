import {
  useCallback,
  useEffect,
  useMemo,
  useState,
} from "react";

import { api } from "../api/client.js";
import { MODULES } from "../config/modules.js";
import { useAuth } from "../context/AuthContext.jsx";
import { useToast } from "../context/ToastContext.jsx";

import DataTable from "../components/DataTable.jsx";
import EntityModal from "../components/EntityModal.jsx";
import Icon from "../components/Icon.jsx";

export default function GenericCrudPage({ moduleId }) {
  const config = MODULES[moduleId];
  const { user } = useAuth();
  const { showToast } = useToast();

  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [modal, setModal] = useState(null);
  const [saving, setSaving] = useState(false);
  const [lookups, setLookups] = useState({});

  if (!config) {
    return (
      <div className="page-header">
        <h1>Module not found</h1>
        <p>No configuration found for: {moduleId}</p>
      </div>
    );
  }

  const resource = api[config.resource];

  const writeAllowed =
    !config.readOnly &&
    (config.writeRoles || []).includes(user.role);

  const deleteAllowed =
    writeAllowed &&
    (config.deleteRoles
      ? config.deleteRoles.includes(user.role)
      : true);

  const load = useCallback(async () => {
    if (!resource) {
      showToast(
        `API resource "${config.resource}" is missing`,
        "error"
      );
      setLoading(false);
      return;
    }

    setLoading(true);

    try {
      const result = await resource.list({
        page: 1,
        pageSize: 100,
        search,
      });

      setRows(result?.items ?? result ?? []);
    } catch (error) {
      console.error(`${config.title} loading failed:`, error);
      showToast(error.message, "error");
    } finally {
      setLoading(false);
    }
  }, [
    resource,
    search,
    showToast,
    config.resource,
    config.title,
  ]);

  useEffect(() => {
    const timer = setTimeout(() => {
      load();
    }, 250);

    return () => clearTimeout(timer);
  }, [load]);

  const fields = useMemo(() => {
    if (modal?.mode === "create") {
      return config.createFields || config.fields || [];
    }

    return config.editFields || config.fields || [];
  }, [config, modal]);

  async function openModal(mode, item = null) {
    const selectedFields =
      mode === "create"
        ? config.createFields || config.fields || []
        : config.editFields || config.fields || [];

    const sources = [
      ...new Set(
        selectedFields
          .filter((field) => field.lookup)
          .map((field) => field.lookup.source)
      ),
    ];

    const nextLookups = {};

    await Promise.all(
      sources.map(async (source) => {
        try {
          const result = await api[source].list({
            page: 1,
            pageSize: 100,
          });

          nextLookups[source] =
            result?.items ?? result ?? [];
        } catch (error) {
          nextLookups[source] = [];

          console.warn(
            `Lookup ${source} unavailable:`,
            error.message
          );
        }
      })
    );

    setLookups(nextLookups);
    setModal({ mode, item });
  }

  async function save(payload) {
    setSaving(true);

    try {
      if (modal.mode === "create") {
        await resource.create(payload);
      } else {
        await resource.update(modal.item.id, payload);
      }

      showToast(
        `${config.title} saved successfully`,
        "success"
      );

      setModal(null);
      await load();
    } catch (error) {
      console.error("Save failed:", error);
      showToast(error.message, "error");
    } finally {
      setSaving(false);
    }
  }

  async function remove(row) {
    const recordName =
      row.name ||
      row.title ||
      row.student ||
      "this record";

    const confirmed = window.confirm(
      `Are you sure you want to delete "${recordName}"?`
    );

    if (!confirmed) return;

    try {
      await resource.remove(row.id);

      setRows((currentRows) =>
        currentRows.filter(
          (item) => item.id !== row.id
        )
      );

      showToast(
        "Record deleted successfully",
        "success"
      );
    } catch (error) {
      console.error("Delete failed:", error);
      showToast(
        `Delete failed: ${error.message}`,
        "error"
      );
    }
  }

  return (
    <>
      <div className="page-header page-header-row">
        <div>
          <h1>{config.title}</h1>
          <p>Live data from the ASP.NET Core Web API</p>
        </div>

        <div className="page-actions">
          {writeAllowed && config.addLabel && (
            <button
              type="button"
              className="btn btn-accent"
              onClick={() => openModal("create")}
            >
              <Icon name="add" />
              {config.addLabel}
            </button>
          )}

          <button
            type="button"
            className="btn btn-outline"
            onClick={load}
          >
            <Icon name="refresh" />
            Refresh
          </button>
        </div>
      </div>

      <div className="table-card">
        <div className="table-toolbar">
          <div className="table-search">
            <Icon name="search" />

            <input
              value={search}
              onChange={(event) =>
                setSearch(event.target.value)
              }
              placeholder={`Search ${config.title.toLowerCase()}...`}
            />
          </div>

          <div className="table-info">
            {rows.length} records
          </div>
        </div>

        <DataTable
          columns={config.columns}
          rows={rows}
          loading={loading}
          canEdit={writeAllowed}
          canDelete={deleteAllowed}
          onEdit={(row) => openModal("edit", row)}
          onDelete={remove}
        />
      </div>

      {modal && (
        <EntityModal
          title={`${
            modal.mode === "create" ? "Add" : "Edit"
          } ${config.title.replace(/s$/, "")}`}
          fields={fields}
          item={modal.item}
          lookups={lookups}
          onClose={() => setModal(null)}
          onSave={save}
          saving={saving}
        />
      )}
    </>
  );
}