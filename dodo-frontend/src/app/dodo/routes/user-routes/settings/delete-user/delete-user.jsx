import { Button, Dialog, ExpandPanel } from "app/components";
import { Input } from "app/components/forms";
import {
	deleteUser as _deleteUser,
	logUserOut as _logUserOut
} from "app/domain/user/actions";
import { email as _email, guid as _guid } from "app/domain/user/selectors";
import { useAction } from "app/hooks/useAction";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useHistory } from "react-router";
import styles from "./delete-user.module.scss";

export const DeleteUser = () => {
	const [dialogOpen, setDialogOpen] = useState(false);
	const [finalDialogOpen, setFinalDialogOpen] = useState(false);
	const [confirmText, setConfirmText] = useState("");
	const [incorrectText, setIncorrectText] = useState(false);
	const [textToShowFullErrorOn, setTextToShowFullErrorOn] = useState("");
	const [otherError, setOtherError] = useState(false);

	const userEmail = useSelector(_email);
	const guid = useSelector(_guid);

	const deleteUser = useAction(_deleteUser);
	const logUserOut = useAction(_logUserOut);

	const history = useHistory();

	const confirmAccountDelete = () => {
		if (confirmText === userEmail) {
			deleteUser(guid, (response) => {
				if (response === false) {
					setOtherError(true);
				} else {
					setDialogOpen(false);
					setFinalDialogOpen(true);
				}
			});
		} else {
			setIncorrectText(true);
			setTextToShowFullErrorOn(confirmText);
		}
	};

	const handleDialogClose = () => {
		setDialogOpen(false);
		setConfirmText("");
		setIncorrectText(false);
		setTextToShowFullErrorOn("");
		setOtherError(false);
	};

	const handleFinalDialogClose = () => {
		logUserOut(() => {
			history.push("/");
		});
	};

	useEffect(() => {
		incorrectText &&
			textToShowFullErrorOn.length &&
			textToShowFullErrorOn !== confirmText &&
			setTextToShowFullErrorOn("");
	}, [confirmText, incorrectText, textToShowFullErrorOn]);

	return (
		<>
			<ExpandPanel
				header={<h2>Danger Zone</h2>}
				headerClassName={styles.dangerZoneTitle}
			>
				<div className={styles.dangerZoneInner}>
					<div>
						This will permanently and irreversibly delete your
						account and all activity associated with it. This action
						cannot be undone.
					</div>
					<Button
						variant="cta-danger"
						onClick={() => setDialogOpen(true)}
					>
						<div>Delete Your Account</div>
					</Button>
				</div>
			</ExpandPanel>

			<Dialog
				fullScreen={false}
				active={dialogOpen}
				close={handleDialogClose}
				title="Are you sure about this?"
				content={
					<div className={styles.modalTextContent}>
						<p>
							You can't undo this once it is confirmed. There is
							no going back.
						</p>
						<p>
							If you still want to say goodbye, enter your full
							email address and click the button below.
						</p>
						<Input
							value={confirmText}
							setValue={setConfirmText}
							maxLength={100}
							placeholder="Enter email address here..."
							error={
								incorrectText &&
								textToShowFullErrorOn === confirmText
							}
							errorJustOnMsg={otherError || incorrectText}
							message={
								(otherError &&
									"Oops, something went wrong! Please try again") ||
								(incorrectText &&
									"Email entered incorrectly. Please try again")
							}
						/>
						<Button
							variant="cta-danger"
							onClick={confirmAccountDelete}
						>
							<div>Confirm Account Delete</div>
						</Button>
					</div>
				}
			/>

			<Dialog
				fullScreen={false}
				active={finalDialogOpen}
				close={handleFinalDialogClose}
				title="Account deleted successfully"
				content={
					<div className={styles.finalModalTextContent}>
						<p>Goodbye for now!</p>
						<p>
							If you want to join us again at any point, just head
							to the Login/Register page and make a new account.
						</p>
					</div>
				}
			/>
		</>
	);
};
