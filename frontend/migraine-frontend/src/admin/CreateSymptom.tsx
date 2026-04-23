import { useEffect, useState } from "react";
import axios from "axios";

interface HealthCondition {
  id: number;
  name: string;
}

// ✅ Enum for migraine phase (matches backend)
const MigrainePhase = {
  BeforeHeadache: 0,
  MigraineWithAura: 1,
  DuringAttack: 2,
  AfterAttack: 3,
} as const;

type MigrainePhaseType =
  (typeof MigrainePhase)[keyof typeof MigrainePhase];

const AssignSymptom = () => {
  const [conditions, setConditions] = useState<HealthCondition[]>([]);
  const [selectedConditionId, setSelectedConditionId] = useState<number | "">("");
  const [symptomName, setSymptomName] = useState("");
  const [description, setDescription] = useState("");
  const [phase, setPhase] = useState<MigrainePhaseType>(
  MigrainePhase.BeforeHeadache
);

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  // ✅ Fetch conditions
  useEffect(() => {
    const fetchConditions = async () => {
      try {
        const res = await axios.get<HealthCondition[]>(
          "http://localhost:5000/api/healthcondition"
        );
        setConditions(res.data);
        console.log("API RESPONSE:", res.data); // 👈 HERE
      } catch (err) {
        console.error("Error loading conditions:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchConditions();
  }, []);

  // ✅ Submit
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!selectedConditionId) {
      alert("Please select a health condition");
      return;
    }

    setSubmitting(true);

    try {
      await axios.post("http://localhost:5000/api/symptom", {
        name: symptomName,
        description: description,
        healthConditionId: selectedConditionId,
        type: String(phase), // ✅ send enum as number
      });

      alert("✅ Symptom assigned successfully!");

      // Reset form
      setSymptomName("");
      setDescription("");
      setSelectedConditionId("");
      setPhase(MigrainePhase.BeforeHeadache);
    } catch (err) {
      console.error(err);
      alert("❌ Error saving symptom");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="container py-5">
      <div className="row justify-content-center">
        <div className="col-md-6">

          <div className="card shadow p-4">
            <h3 className="text-center mb-4">Assign Symptom</h3>

            {loading ? (
              <p className="text-center">Loading conditions...</p>
            ) : (
              <form onSubmit={handleSubmit}>

                {/* Symptom Name */}
                <div className="mb-3">
                  <label className="form-label">Symptom Name</label>
                  <input
                    type="text"
                    className="form-control"
                    value={symptomName}
                    onChange={(e) => setSymptomName(e.target.value)}
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
                    value={selectedConditionId}
                    onChange={(e) => {
                      const value = e.target.value;
                      setSelectedConditionId(value === "" ? "" : Number(value));
                    }}
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

                {/* ✅ Migraine Phase */}
                <div className="mb-4">
                  <label className="form-label">Migraine Phase</label>
                  <select
                    className="form-select"
                    value={phase}
                    onChange={(e) => setPhase(Number(e.target.value) as MigrainePhaseType)}
                  >
                    <option value={MigrainePhase.BeforeHeadache}>
                      Before Headache
                    </option>
                    <option value={MigrainePhase.MigraineWithAura}>
                      Migraine with Aura
                    </option>
                    <option value={MigrainePhase.DuringAttack}>
                      During Attack
                    </option>
                    <option value={MigrainePhase.AfterAttack}>
                      After Attack
                    </option>
                  </select>
                </div>

                {/* Submit */}
                <button
                  type="submit"
                  className="btn btn-primary w-100"
                  disabled={submitting}
                >
                  {submitting ? "Saving..." : "Assign Symptom"}
                </button>

              </form>
            )}
          </div>

        </div>
      </div>
    </div>
  );
};

export default AssignSymptom;