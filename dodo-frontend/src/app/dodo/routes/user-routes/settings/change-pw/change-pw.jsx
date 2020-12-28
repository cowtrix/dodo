import { Input, Submit } from 'app/components/forms';
import React, { useState } from 'react';

import styles from './change-pw.module.scss';

export const ChangePw = () => {

	const [currentPw, setCurrentPw] = useState('');
	const [newPw, setNewPw] = useState('');
	const [confirmNewPw, setConfirmNewPw] = useState('');

  return (
    <div className={styles.warning}>
      <h3 className={styles.h3Title}>Change your password</h3>
      <Input
        id="current-pw"
        type="password"
        placeholder="Current password..."
        value={currentPw}
        setValue={setCurrentPw}
      />
      <Input
        id="new-pw"
        type="password"
        placeholder="New password..."
        value={newPw}
        setValue={setNewPw}
      />
      <Input
        id="confirm-new-pw"
        type="password"
        placeholder="Confirm password..."
        value={confirmNewPw}
        setValue={setConfirmNewPw}
      />
      <Submit
        value="change Password"
        submit={()=>{}}
      />
    </div>
  )
}