import StatusBadge from "./StatusBadge.jsx";
import Icon from "./Icon.jsx";

function formatValue(value, type) {
  if (
    value === null ||
    value === undefined ||
    value === ""
  ) {
    return "—";
  }

  if (type === "currency") {
    return new Intl.NumberFormat("en-IN", {
      style: "currency",
      currency: "INR",
      maximumFractionDigits: 0,
    }).format(Number(value));
  }

  if (type === "date") {
    const date = new Date(`${value}T00:00:00`);

    return Number.isNaN(date.getTime())
      ? String(value)
      : date.toLocaleDateString("en-IN");
  }

  if (type === "datetime") {
    const date = new Date(value);

    return Number.isNaN(date.getTime())
      ? String(value)
      : date.toLocaleString("en-IN");
  }

  return String(value);
}

export default function DataTable({
  columns = [],
  rows = [],
  loading = false,
  canEdit = false,
  canDelete = false,
  onEdit,
  onDelete,
}) {
  const normalizedColumns = columns.map((column) => {
    if (Array.isArray(column)) {
      return {
        key: column[0],
        label: column[1],
        type: column[2],
      };
    }

    return {
      key: column.key,
      label: column.label,
      type: column.type,
    };
  });

  const showActions = canEdit || canDelete;

  return (
    <div className="table-scroll">
      <table className="data-table">
        <thead>
          <tr>
            {normalizedColumns.map((column) => (
              <th key={column.key}>
                {column.label}
              </th>
            ))}

            {showActions && <th>Actions</th>}
          </tr>
        </thead>

        <tbody>
          {loading ? (
            <tr>
              <td
                className="empty-cell"
                colSpan={
                  normalizedColumns.length +
                  (showActions ? 1 : 0)
                }
              >
                Loading records...
              </td>
            </tr>
          ) : rows.length === 0 ? (
            <tr>
              <td
                className="empty-cell"
                colSpan={
                  normalizedColumns.length +
                  (showActions ? 1 : 0)
                }
              >
                No records found
              </td>
            </tr>
          ) : (
            rows.map((row, rowIndex) => (
              <tr key={row.id || rowIndex}>
                {normalizedColumns.map((column) => {
                  const value = row[column.key];

                  return (
                    <td
                      key={column.key}
                      className={
                        column.key === "name" ||
                        column.key === "title" ||
                        column.key === "student"
                          ? "primary-cell"
                          : ""
                      }
                    >
                      {column.type === "status" ? (
                        <StatusBadge value={value} />
                      ) : column.type === "role" ? (
                        <span
                          className={`role-tag ${String(
                            value || ""
                          ).toLowerCase()}`}
                        >
                          {formatValue(value)}
                        </span>
                      ) : (
                        formatValue(
                          value,
                          column.type
                        )
                      )}
                    </td>
                  );
                })}

                {showActions && (
                  <td>
                    <div className="row-actions">
                      {canEdit && (
                        <button
                          type="button"
                          className="action-btn"
                          title="Edit"
                          onClick={() => onEdit?.(row)}
                        >
                          <Icon name="edit" />
                        </button>
                      )}

                      {canDelete && (
                        <button
                          type="button"
                          className="action-btn danger"
                          title="Delete"
                          onClick={() =>
                            onDelete?.(row)
                          }
                        >
                          <Icon name="delete" />
                        </button>
                      )}
                    </div>
                  </td>
                )}
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}