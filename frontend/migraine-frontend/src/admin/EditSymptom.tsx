import { useEffect, useState } from "react";
import axios from "axios";
import { useParams, useNavigate } from "react-router-dom";

interface HealthCondition {
  id: number;
  name: string;
}

interface SymptomDto {
  id: number;
  name: string;
  description: string;
  healthConditionId: number;
  type: number;
}

const MigrainePhase = {
  BeforeHeadache: "BeforeHeadache",
  MigraineWithAura: "MigraineWithAura",
  DuringAttack: "DuringAttack",
  AfterAttack: "AfterAttack"
};

const EditSymptom = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [conditions, setConditions] = useState<HealthCondition[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [healthConditionId, setHealthConditionId] = useState<number | "">("");
  //const [type, setType] = useState<number>(0);
const [type, setType] = useState<string>(MigrainePhase.BeforeHeadache);
  // Load conditions + symptom
  useEffect(() => {
        if (!id || id === "0") {
            console.error("Invalid ID:", id);
            return;
        }

    const fetchData = async () => {
      try {
        const [conditionsRes, symptomRes] = await Promise.all([
          //axios.get<HealthCondition[]>("http://localhost:5000/api/healthcondition"),
          axios.get<HealthCondition[]>("/api/healthcondition"),
          //axios.get<SymptomDto>(`http://localhost:5000/api/symptom/${id}`)
          axios.get<SymptomDto>(`/api/symptom/${id}`)
        ]);
        console.log(symptomRes.data);
        setConditions(conditionsRes.data);

        const symptom = symptomRes.data;
        setName(symptom.name);
        setDescription(symptom.description);
        setHealthConditionId(symptom.healthConditionId);
        setType(symptom.type.toString());

      } catch (err) {
        console.error("Error loading data", err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id]);

  // Update
  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!healthConditionId) {
      alert("Please select a health condition");
      return;
    }

    setSaving(true);

    try {
      const token = localStorage.getItem("token");
      //await axios.put(`http://localhost:5000/api/symptom/${id}`, {
      await axios.put(`/api/symptom/${id}`, {
        name,
        description,
        healthConditionId: Number(healthConditionId),
        type
      },
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    );

      alert("Symptom updated successfully!");
      navigate("/");
    } catch (err) {
      console.error("Update failed", err);
      alert("Failed to update symptom");
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return <div className="container py-5">Loading...</div>;
  }

  return (
    <div className="container py-5">
      <div className="row justify-content-center">
        <div className="col-md-6">

          <div className="card shadow p-4">
            <h3 className="text-center mb-4">Edit Symptom</h3>

            <form onSubmit={handleUpdate}>

              {/* Name */}
              <div className="mb-3">
                <label className="form-label">Symptom Name</label>
                <input
                  className="form-control"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  required
                />
              </div>

              {/* Description */}
              <div className="mb-3">
                <label className="form-label">Description</label>
                <textarea
                  className="form-control"
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  required
                />
              </div>

              {/* Health Condition */}
              <div className="mb-3">
                <label className="form-label">Health Condition</label>
                <select
                  className="form-select"
                  value={healthConditionId}
                  onChange={(e) =>
                    setHealthConditionId(Number(e.target.value))
                  }
                  required
                >
                  <option value="">-- Select Condition --</option>

                  {conditions.map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.name}
                    </option>
                  ))}
                </select>
              </div>

              {/* Phase */}
              <div className="mb-4">
                <label className="form-label">Migraine Phase</label>
                <select className="form-select"
                    value={type}
                    onChange={(e) => setType(e.target.value)}
                    >
                    <option value="BeforeHeadache">Before Headache</option>
                    <option value="MigraineWithAura">Migraine With Aura</option>
                    <option value="DuringAttack">During Attack</option>
                    <option value="AfterAttack">After Attack</option>
                    </select>
              </div>

              {/* Button */}
              <button
                type="submit"
                className="btn btn-primary w-100"
                disabled={saving}
              >
                {saving ? "Updating..." : "Update Symptom"}
              </button>

            </form>
          </div>

        </div>
      </div>
    </div>
  );
};

export default EditSymptom;